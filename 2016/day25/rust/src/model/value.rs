use std::{str::FromStr, fmt};

use super::register::*;

#[derive(Debug)]
pub enum Value {
    Immediate(i32),
    Register(Register),
}

impl fmt::Display for Value {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            Value::Register(reg) => write!(f, "{reg}"),
            Value::Immediate(imm) => write!(f, "{imm}"),
        }
    }
}

#[derive(Debug)]
pub struct ParseValueError(String);

impl FromStr for Value {
    type Err = ParseValueError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if let Ok(reg) = s.parse::<Register>() {
            return Ok(Value::Register(reg));
        }
        if let Ok(imm) = s.parse::<i32>() {
            return Ok(Value::Immediate(imm));
        }
        return Err(ParseValueError(s.to_string()));
    }
}

