use std::{fmt, str::FromStr};

use super::register::*;
use super::value::*;

#[derive(Debug)]
pub enum Instruction {
    Copy {
        src: Value,
        dest: Register,
    },
    Increase {
        reg: Register,
    },
    Decrease {
        reg: Register,
    },
    JumpIfNotZero {
        condition: Value,
        offset: i32,
    },
}

impl fmt::Display for Instruction {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            Instruction::Copy { src, dest } => write!(f, "cpy {src} {dest}"),
            Instruction::Increase { reg } => write!(f, "inc {reg}"),
            Instruction::Decrease { reg } => write!(f, "dec {reg}"),
            Instruction::JumpIfNotZero { condition, offset } => write!(f, "jnz {condition} {offset}"),
        }
    }
}

#[derive(Debug, PartialEq, Eq)]
pub struct ParseInstructionError(String);

impl FromStr for Instruction {
    type Err = ParseInstructionError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {

        let parts: Vec<&str> = s.split(' ').collect();

        match parts.as_slice() {
            ["cpy", src, dest] => {
                Ok(Instruction::Copy {
                    src: src.parse().unwrap(),
                    dest: dest.parse().unwrap(),
                })
            },

            ["inc", reg] => {
                Ok(Instruction::Increase {
                    reg: reg.parse().unwrap()
                })
            }

            ["dec", reg] => {
                Ok(Instruction::Decrease {
                    reg: reg.parse().unwrap()
                })
            }

            ["jnz", condition, offset] => {
                Ok(Instruction::JumpIfNotZero {
                    condition: condition.parse().unwrap(),
                    offset: offset.parse().unwrap(),
                })
            }

            _ => Err(ParseInstructionError(s.to_string())),
        }
    }
}

