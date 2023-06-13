use std::{str::FromStr, fmt};

#[derive(Debug, Clone, Copy)]
pub enum Register {
    A = 0,
    B = 1,
    C = 2,
    D = 3,
}

impl fmt::Display for Register {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            Register::A => write!(f, "a"),
            Register::B => write!(f, "b"),
            Register::C => write!(f, "c"),
            Register::D => write!(f, "d"),
        }
    }
}

#[derive(Debug, PartialEq, Eq)]
pub struct ParseRegisterError(String);

impl FromStr for Register {
    type Err = ParseRegisterError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "a" => Ok(Register::A),
            "b" => Ok(Register::B),
            "c" => Ok(Register::C),
            "d" => Ok(Register::D),
            _ => Err(ParseRegisterError(s.to_string())),
        }
    }
}

