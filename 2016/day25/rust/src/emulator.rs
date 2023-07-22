use super::model::value::*;
use super::model::instruction::*;

#[derive(Debug)]
struct State {
    pc: usize,
    regs: [i32; 4],
    prev_out_value: Option<i32>,
    clock_valid: bool,
    clock_cycles: usize,
}

impl State {
    fn new(initial_regs: [i32; 4]) -> State {
        State {
            pc: 0,
            regs: initial_regs,
            prev_out_value: None,
            clock_valid: true,
            clock_cycles: 0,
        }
    }

    fn get_value(&self, value: &Value) -> i32 {
        match value {
            Value::Register(reg) => {
                self.regs[*reg as usize]
            },
            Value::Immediate(imm) => {
                *imm
            }
        }
    }

    fn execute(&mut self, instruction: &Instruction) {
        let mut new_pc: usize = self.pc + 1;

        match instruction {
            Instruction::Copy { src, dest } => {
                let dest = *dest as usize;
                self.regs[dest] = self.get_value(src);
            },
            Instruction::Increase { reg } => {
                let reg = *reg as usize;
                self.regs[reg] += 1;
            },
            Instruction::Decrease { reg } => {
                let reg = *reg as usize;
                self.regs[reg] -= 1;
            },
            Instruction::JumpIfNotZero { condition, offset } => {
                let offset = *offset;
                if self.get_value(condition) != 0 {
                    if offset > 0 {
                        new_pc = self.pc + (offset) as usize;
                    } else {
                        new_pc = self.pc - (-offset) as usize;
                    }
                }
            },
            Instruction::Out { value } => {
                let v = self.get_value(value);
                let pv = self.prev_out_value.unwrap_or(-1);
                if v != 0 && v != 1 || v == pv {
                    self.clock_valid = false;
                }
                self.clock_cycles += 1;
                self.prev_out_value = Some(v);
            },
        }

        self.pc = new_pc;
    }
}

pub fn emulate_clock_instructions(instructions: &[Instruction], initial_regs: [i32;4]) -> bool {
    const TARGET_CLOCK_CYCLES: usize = 1000 * 10;
    
    let mut state = State::new(initial_regs);

    loop {
        let current_instr = &instructions[state.pc as usize];
        state.execute(current_instr);

        if state.clock_valid == false ||
            state.clock_cycles >= TARGET_CLOCK_CYCLES ||
            state.pc as usize >= instructions.len() {
            break;
        }
    }

    return state.clock_valid && state.clock_cycles >= TARGET_CLOCK_CYCLES;
}

