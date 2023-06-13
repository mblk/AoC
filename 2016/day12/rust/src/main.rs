mod model;
mod parser;
mod emulator;
mod x86_jit;

use emulator::emulate_instructions;
use x86_jit::{translate_to_x86_opcodes, execute_code_in_buffer_unsafe};

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();
    let instructions = parser::parse_program(filename);
    let part1_init = [0,0,0,0];
    let part2_init = [0,0,1,0];

    let part1_emulated = emulate_instructions(&instructions, part1_init);
    let part2_emulated = emulate_instructions(&instructions, part2_init);
    println!("Part1 emulated: {part1_emulated:?}");
    println!("Part2 emulated: {part2_emulated:?}");

    let x86_instructions_part1 = translate_to_x86_opcodes(&instructions, part1_init);
    let x86_instructions_part2 = translate_to_x86_opcodes(&instructions, part2_init);
    let part1_executed = execute_code_in_buffer_unsafe(&x86_instructions_part1);
    let part2_executed = execute_code_in_buffer_unsafe(&x86_instructions_part2);
    println!("Part1 executed: {part1_executed:?}");
    println!("Part2 executed: {part2_executed:?}");

    let speedup1 = part1_emulated.1.as_secs_f64() / part1_executed.1.as_secs_f64();
    let speedup2 = part2_emulated.1.as_secs_f64() / part2_executed.1.as_secs_f64();
    println!("speedup1 {speedup1:.1}");
    println!("speedup2 {speedup2:.1}");
}
