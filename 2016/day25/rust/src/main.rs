mod model;
mod parser;
mod emulator;

use emulator::emulate_clock_instructions;
use model::instruction::Instruction;

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();

    let instructions = match parser::parse_program(&filename) {
        Ok(instr) => instr,
        Err(err) => {
            eprintln!("{:?}", err);
            return;
        }
    };

    println!("Part1: {}", find_clock_signal_configuration(&instructions));
}

fn find_clock_signal_configuration(instructions: &[Instruction]) -> i32 {
    let mut starting_a: i32 = 0;
    loop {
        let is_clock_signal = emulate_clock_instructions(&instructions, [starting_a,0,0,0]);
        if is_clock_signal {
            return starting_a;
        }
        starting_a += 1;
    }
}
