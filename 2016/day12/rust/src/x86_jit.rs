use std::time::{Instant, Duration};

use crate::model::register::*;
use crate::model::value::*;
use crate::model::instruction::*;

#[derive(Debug)]
struct X86Instruction<'a> {
    instruction: & 'a Instruction,
    opcodes: Vec<u8>,
}

fn get_x86_reg_bits(register: &Register) -> u8 {
    match register {
        Register::A => 0, // used in many instruction to encode src/dest-reg
        Register::B => 3,
        Register::C => 1,
        Register::D => 2,
    }
}

fn emit_x86_move_reg_to_reg(buf: &mut Vec<u8>, src: &Register, dest: &Register) {
    // mov r, r
    buf.push(0x48);
    buf.push(0x89);
    buf.push(0b11_000_000 | (get_x86_reg_bits(src)<<3) | get_x86_reg_bits(dest));
}

fn emit_x86_move_imm_to_reg(buf: &mut Vec<u8>, register: &Register, imm: i32) {
    let v = imm as u32;
    // mov r, imm
    buf.push(0b10_111_000 | get_x86_reg_bits(register));
    buf.push((v & 0xFF) as u8);
    buf.push(((v >> 8) & 0xFF) as u8);
    buf.push(((v >> 16) & 0xFF) as u8);
    buf.push(((v >> 24) & 0xFF) as u8);
}

fn emit_x86_move(buf: &mut Vec<u8>, src: &Value, dest: &Register) {
    match src {
        Value::Register(src_reg) => emit_x86_move_reg_to_reg(buf, src_reg, dest),
        Value::Immediate(src_imm) => emit_x86_move_imm_to_reg(buf, dest, *src_imm),
    }
}

fn emit_x86_inc_reg(buf: &mut Vec<u8>, reg: &Register) {
    // inc r
    buf.push(0x48);
    buf.push(0xFF);
    buf.push(0b1100_0_000 | get_x86_reg_bits(reg));
}

fn emit_x86_dec_reg(buf: &mut Vec<u8>, reg: &Register) {
    // dec r
    buf.push(0x48);
    buf.push(0xFF);
    buf.push(0b1100_1_000 | get_x86_reg_bits(reg));
}

fn emit_x86_nop(buf: &mut Vec<u8>) {
    // nop
    buf.push(0x90);
}

fn emit_x86_rel_jump(buf: &mut Vec<u8>) {
    // jmp offset
    buf.push(0xEB);
    buf.push(0x00); // set in second pass.
}

fn emit_x86_rel_jump_if_reg_not_zero(buf: &mut Vec<u8>, reg: &Register) {
    // cmp r, 0
    buf.push(0x48);
    buf.push(0x83);
    buf.push(0b11111_000 | get_x86_reg_bits(reg));
    buf.push(0x00);

    // jne offset
    buf.push(0x75);
    buf.push(0x00); // set in second pass.
}

fn emit_x86_jump_if_not_zero(buf: &mut Vec<u8>, condition: &Value) {
    match condition {
        Value::Immediate(imm) if *imm == 0 => emit_x86_nop(buf),
        Value::Immediate(_) => emit_x86_rel_jump(buf),
        Value::Register(reg) => emit_x86_rel_jump_if_reg_not_zero(buf, reg),
    }
}

fn emit_x86_opcode(buf: &mut Vec<u8>, instr: &Instruction) {
    match instr {
        Instruction::Copy { src, dest } => emit_x86_move(buf, src, dest),
        Instruction::Increase { reg } => emit_x86_inc_reg(buf, reg),
        Instruction::Decrease { reg } => emit_x86_dec_reg(buf, reg),
        Instruction::JumpIfNotZero { condition, offset: _ } => emit_x86_jump_if_not_zero(buf, condition),
    };
}

fn emit_x86_push_reg(buf: &mut Vec<u8>, reg: &Register) {
    // push r
    buf.push(0b0101_0_000 | get_x86_reg_bits(reg));
}

fn emit_x86_pop_reg(buf: &mut Vec<u8>, reg: &Register) {
    // pop r
    buf.push(0b0101_1_000 | get_x86_reg_bits(reg));
}

fn emit_x86_create_stack_frame(buf: &mut Vec<u8>, initial_regs: [i32; 4]) {
    // push rbp
    buf.push(0x55);
    // mov rbp, rsp
    buf.push(0x48); buf.push(0x89); buf.push(0xE5);
    // save values in b/c/d
    emit_x86_push_reg(buf, &Register::B);
    emit_x86_push_reg(buf, &Register::C);
    emit_x86_push_reg(buf, &Register::D);
    // initialize a,b,c,d
    emit_x86_move_imm_to_reg(buf, &Register::A, initial_regs[0]);
    emit_x86_move_imm_to_reg(buf, &Register::B, initial_regs[1]);
    emit_x86_move_imm_to_reg(buf, &Register::C, initial_regs[2]);
    emit_x86_move_imm_to_reg(buf, &Register::D, initial_regs[3]);
}

fn emit_x86_destroy_stack_frame(buf: &mut Vec<u8>) {
    // restore values in b/c/d
    emit_x86_pop_reg(buf, &Register::D);
    emit_x86_pop_reg(buf, &Register::C);
    emit_x86_pop_reg(buf, &Register::B);
    // pop rbp
    buf.push(0x5D);
    // ret
    buf.push(0xC3);
}

fn create_x86_instruction(instr: &Instruction) -> X86Instruction {
    let mut buf: Vec<u8> = Vec::new();
    emit_x86_opcode(&mut buf, instr);

    return X86Instruction {
        instruction: instr,
        opcodes: buf,
    };
}

pub fn translate_to_x86_opcodes(instructions: &[Instruction], initial_regs: [i32; 4]) -> Vec<u8> {
    let mut x86_instructions: Vec<X86Instruction> = Vec::new();

    // First pass (relative jump offsets missing)
    for instr in instructions {
        x86_instructions.push(create_x86_instruction(instr));
    }

    // Second pass: fill in missing offsets
    for index in 0..x86_instructions.len() {
        let instr = &x86_instructions[index];

        match instr.instruction {
            Instruction::JumpIfNotZero { condition: _, offset } => {
                let offset = *offset;
                // -1=jmp to prev, 0=jump to current, 1=jump to next; 2=skip next

                // TODO simplify into one loop?
                let mut relative_addr: i32 = 0;
                if offset < 1 {
                    for i in offset..0 {
                        let prev_index = index - (-i) as usize;
                        let prev_instr_len = x86_instructions[prev_index].opcodes.len();
                        relative_addr -= prev_instr_len as i32;
                    }
                } 
                else if offset >= 1 {
                    for i in 0..offset {
                        let next_index = index + i as usize;
                        let next_instr_len = x86_instructions[next_index].opcodes.len();
                        relative_addr += next_instr_len as i32;
                    }
                }

                // Offset in instruction is relative to current instruction.
                // Offset in x86-opcode is relative to next instruction.
                relative_addr -= instr.opcodes.len() as i32;
                assert!(-128 <= relative_addr && relative_addr <= 127, "offset too far away for short jump");

                let last_opcode_idx = x86_instructions[index].opcodes.len() - 1;
                x86_instructions[index].opcodes[last_opcode_idx] = relative_addr as u8;
            },
            _ => {},
        }
    }

    let mut opcodes: Vec<u8> = Vec::new();

    // Create stack frame, save b/c/d and set initial values for a/b/c/d.
    emit_x86_create_stack_frame(&mut opcodes, initial_regs);

    for x86_instr in &x86_instructions {
        opcodes.extend_from_slice(&x86_instr.opcodes);
    }

    // Restore b/c/d, destroy stack frame and return.
    emit_x86_destroy_stack_frame(&mut opcodes);

    return opcodes;
}

pub fn execute_code_in_buffer_unsafe(buf: &[u8]) -> (i32, Duration) {
    // TODO error handling?
    let mut executable_memory = region::alloc(buf.len(), region::Protection::READ_WRITE_EXECUTE).unwrap();

    unsafe {
        // memcpy
        std::ptr::copy_nonoverlapping(buf.as_ptr(), executable_memory.as_mut_ptr(), buf.len());
    }

    // use a type alias to make the following declaration slightly easier to read.
    type FnPtrType = extern "C" fn() -> i32;

    let fn_ptr: FnPtrType = unsafe {
        std::mem::transmute(executable_memory.as_ptr::<u8>())
    };

    let start_time = Instant::now();
    let ret = fn_ptr();
    let run_time = start_time.elapsed();

    return (ret, run_time);
}

