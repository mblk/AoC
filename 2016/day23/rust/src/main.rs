mod model;
mod parser;
mod emulator;

use emulator::emulate_instructions;

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();
    
    match parser::parse_program(filename) {

        Err(err) => {
            eprintln!("{}", err.0);
        },

        Ok(instructions) => {
            //dbg!(&instructions);
            
            let part1_init = [7,0,0,0];
            let part1_emulated = emulate_instructions(&instructions, part1_init);
            println!("Part1: {part1_emulated:?}");

            let part2_init = [12,0,0,0];
            let part2_emulated = emulate_instructions(&instructions, part2_init);
            println!("Part2: {part2_emulated:?}");
        },
    }
}
