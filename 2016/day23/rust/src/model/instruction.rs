use std::{fmt, str::FromStr};

use super::register::*;
use super::value::*;

#[derive(Debug, Clone, Copy)]
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
        offset: Value,
    },
    Toggle {
        offset: Value,
    },
}

impl fmt::Display for Instruction {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            Instruction::Copy { src, dest } => write!(f, "cpy {src} {dest}"),
            Instruction::Increase { reg } => write!(f, "inc {reg}"),
            Instruction::Decrease { reg } => write!(f, "dec {reg}"),
            Instruction::JumpIfNotZero { condition, offset } => write!(f, "jnz {condition} {offset}"),
            Instruction::Toggle { offset } => write!(f, "tgl {offset}"),
        }
    }
}

#[derive(Debug, PartialEq, Eq)]
pub struct ParseInstructionError(pub String);

impl FromStr for Instruction {
    type Err = ParseInstructionError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {

        fn parse_reg(s: &str) -> Result<Register, ParseInstructionError> {
            s.parse().map_err(|_| ParseInstructionError(format!("invalid register '{s}'")))
        }

        fn parse_value(s: &str) -> Result<Value, ParseInstructionError> {
            s.parse().map_err(|_| ParseInstructionError(format!("invalid value '{s}'")))
        }

        //fn parse_imm(s: &str) -> Result<i32, ParseInstructionError> {
        //    s.parse().map_err(|_| ParseInstructionError(format!("invalid immediate '{s}'")))
        //}

        match s.split(' ').collect::<Vec<&str>>().as_slice() {

            ["cpy", src, dest] => Ok(Instruction::Copy {
                    src: parse_value(src)?,
                    dest: parse_reg(dest)?,
                }),

            ["inc", reg] => Ok(Instruction::Increase {
                    reg: parse_reg(reg)?
                }),

            ["dec", reg] => Ok(Instruction::Decrease {
                    reg: parse_reg(reg)?
                }),

            ["jnz", condition, offset] => Ok(Instruction::JumpIfNotZero {
                    condition: parse_value(condition)?,
                    offset: parse_value(offset)?,
                }),

            ["tgl", offset] => Ok(Instruction::Toggle {
                    offset: parse_value(offset)?,
                }),
                
            _ => Err(ParseInstructionError(s.to_string())),
        }
    }
}

