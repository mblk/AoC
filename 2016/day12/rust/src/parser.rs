use crate::model::instruction::Instruction;

pub fn parse_program(filename: String) -> Vec<Instruction> {

    let file_data = std::fs::read_to_string(filename);
    if let Err(err) = file_data {
        eprintln!("Can't read file: {err}");
        panic!();
    }

    let instructions: Vec<Instruction> = file_data
        .unwrap()
        .lines()
        .map(|line| line.parse().unwrap())
        .collect();

    return instructions;
}

