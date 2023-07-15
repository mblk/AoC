use crate::model::instruction::{Instruction, ParseInstructionError};

#[derive(Debug)]
pub struct ProgramParseError(pub String);

pub fn parse_program(filename: String) -> Result<Vec<Instruction>, ProgramParseError> {

    let file_data = std::fs::read_to_string(&filename);
    if let Err(err) = file_data {
        return Err(ProgramParseError(format!("Can't read file '{}': {}", &filename, err.to_string())));
    }

    let instructions: Result<Vec<Instruction>, ProgramParseError> = file_data
        .unwrap()
        .lines()
        .map(|line| line
             .parse()
             .map_err(|e:ParseInstructionError| ProgramParseError(format!("Syntax error in line '{}': {}", line, e.0))))
        .collect();

    return instructions;
}

