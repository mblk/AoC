use std::time::{Instant, Duration};

use super::model::value::*;
use super::model::instruction::*;

#[derive(Debug)]
struct State {
    pc: usize,
    regs: [i32; 4],
}

impl State {
    fn new(initial_regs: [i32; 4]) -> State {
        State {
            pc: 0,
            regs: initial_regs,
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

    fn add_offset(value: usize, offset: i32) -> usize {
        if offset > 0 {
            value + (offset) as usize
        } else {
            value - (-offset) as usize
        }
    }

    fn execute(&mut self, instructions: &mut [Instruction]) {
        let current_instruction = &instructions[self.pc];
        let mut new_pc: usize = self.pc + 1;

        match current_instruction {
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
                let offset = self.get_value(offset);
                if self.get_value(condition) != 0 {
                    new_pc = Self::add_offset(self.pc, offset);
                }
            },
            Instruction::Toggle { offset } => {
                let offset = self.get_value(offset);
                let target_addr = Self::add_offset(self.pc, offset);
                if target_addr < instructions.len() {
                    instructions[target_addr] = match instructions[target_addr] {
                        Instruction::Increase { reg } => Instruction::Decrease { reg, },
                        Instruction::Decrease { reg } => Instruction::Increase { reg, },
                        Instruction::Toggle { offset } => Instruction::Increase {
                            reg: match offset {
                                Value::Register(r) => r,
                                _ => panic!("offset is not a reg"),
                            },
                        },
                        Instruction::JumpIfNotZero { condition, offset } => Instruction::Copy {
                            src: condition,
                            dest: match offset {
                                Value::Register(r) => r,
                                _ => panic!("offset is not a reg"),
                            },
                        },
                        Instruction::Copy { src, dest } => Instruction::JumpIfNotZero {
                            condition: src,
                            offset: Value::Register(dest),
                        },
                    }
                }
            },
        }

        self.pc = new_pc;
    }
}

pub fn emulate_instructions(instructions: &[Instruction], initial_regs: [i32;4]) -> (i32, Duration) {
    let start_time = Instant::now();

    let mut mut_instructions: Vec<Instruction> = instructions.iter()
        .map(|instr| *instr)
        .collect();

    let mut state = State::new(initial_regs);

    while (state.pc as usize) < mut_instructions.len() {
        state.execute(&mut mut_instructions);
    }

    return (state.regs[0], start_time.elapsed());
}

