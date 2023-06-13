use std::time::{Instant, Duration};

use super::model::value::*;
//use super::model::register::*;
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
        }

        self.pc = new_pc;
    }
}

pub fn emulate_instructions(instructions: &[Instruction], initial_regs: [i32;4]) -> (i32, Duration) {
    let start_time = Instant::now();
    let mut state = State::new(initial_regs);
    //let mut ticks: u32 = 0;

    loop {
        let current_instr = &instructions[state.pc as usize];
        state.execute(current_instr);
        //ticks += 1;

        if state.pc as usize >= instructions.len() {
            break;
        }
    }

    let run_time = start_time.elapsed();
    //println!("Finished after {run_time:?}, {ticks} ticks in {state:?}");

    return (state.regs[0], run_time);
}

