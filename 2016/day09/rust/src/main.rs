use std::{env, fs, fmt};

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file path");
        return;
    }

    let filename = env::args().nth(1).unwrap();

    match fs::read_to_string(filename) {
        Result::Err(err) => {
            eprintln!("Error: {err}");
            return;
        }
        Result::Ok(content) => {
            let compressed_message = content.trim();
            //for format in CompressionFormat::iter() {
            for format in &[CompressionFormat::Version1, CompressionFormat::Version2] {
                let decompressed_length = decompress(compressed_message, format);
                println!("{format}: {decompressed_length}");
            }
        },
    }
}

enum CompressionFormat {
    Version1,
    Version2,
}

impl fmt::Display for CompressionFormat {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            CompressionFormat::Version1 => write!(f, "V1.0"),
            CompressionFormat::Version2 => write!(f, "V2.0"),
        }
    }
}

fn decompress(input: &str, format: &CompressionFormat) -> usize {
    let mut total_length: usize = 0;
    let mut current_position: &str = input;

    while current_position.len() > 0 {

        let advance_count: usize = match try_get_command(current_position) {

            // Command at the current position?
            Some((cmd_len, rep_count, cmd_argument)) => {
                total_length += rep_count * match format {
                    CompressionFormat::Version1 => cmd_argument.len(),
                    CompressionFormat::Version2 => decompress(cmd_argument, format),
                };
                cmd_len
            },

            // Not a command, just a regular character.
            None => {
                total_length += 1;
                1
            },
        };

        current_position = &current_position[advance_count..];
    }

    return total_length;
}

/// Tries to parse commands in the following schema:
/// (AxB)C...
/// with:
/// A = length of the command argument
/// B = repetition count
/// C = command argument
/// . = following characters which are not part of the command
///
/// Returns:
/// - Total length of command in characters (including brackets and argument)
/// - Repetition count (number following 'x')
/// - Command argument (characters following the closing bracket)
fn try_get_command(input: &str) -> Option<(usize,usize,&str)> {

    if input.len() < 5 ||
       input.chars().nth(0).unwrap() != '(' {
        return None;
    }

    let end_idx = input.chars().position(|c| c == ')').unwrap();
    let cmd_str = &input[1..end_idx];
    let sep_idx = cmd_str.chars().position(|c| c == 'x').unwrap();

    let arg1 = &cmd_str[0..sep_idx];
    let arg2 = &cmd_str[sep_idx+1..];

    let char_count: usize = arg1.parse().unwrap();
    let rep_count: usize = arg2.parse().unwrap();

    let cmd_len = cmd_str.len() + 2;

    let cmd_argument = &input[cmd_len .. cmd_len+char_count];

    return Some((cmd_len + cmd_argument.len(), rep_count, cmd_argument));
}

#[test]
fn test_decompress_v1() {

    fn decompress_v1(input: &str) -> usize {
        decompress(input, &CompressionFormat::Version1)
    }

    assert_eq!("ADVENT".len(), decompress_v1("ADVENT"));
    assert_eq!("ABBBBBC".len(), decompress_v1("A(1x5)BC"));
    assert_eq!("XYZXYZXYZ".len(), decompress_v1("(3x3)XYZ"));
    assert_eq!("ABCBCDEFEFG".len(), decompress_v1("A(2x2)BCD(2x2)EFG"));
    assert_eq!("(1x3)A".len(), decompress_v1("(6x1)(1x3)A"));
    assert_eq!("X(3x3)ABC(3x3)ABCY".len(), decompress_v1("X(8x2)(3x3)ABCY"));
}

#[test]
fn test_decompress_v2() {

    fn decompress_v2(input: &str) -> usize {
        decompress(input, &CompressionFormat::Version2)
    }

    assert_eq!(9, decompress_v2("(3x3)XYZ"));
    assert_eq!(20, decompress_v2("X(8x2)(3x3)ABCY"));
    assert_eq!(241920, decompress_v2("(27x12)(20x12)(13x14)(7x10)(1x12)A"));
    assert_eq!(445, decompress_v2("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN"));
}


