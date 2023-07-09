use std::str::FromStr;

#[derive(Debug, Clone, Copy)]
enum Instruction {
    SwapPosition(usize, usize),
    SwapLetter(char, char),
    RotateRight(isize),
    RotateOnLetter(char),
    Reverse(usize, usize),
    Move(usize, usize),
}

#[derive(Debug)]
enum ParseInstructionError {
    InvalidInstruction(String),
    BadValue,
}

impl Instruction {
    fn parse_value<T: FromStr>(s: &str) -> Result<T, ParseInstructionError> {
        s.parse().map_err(|_| ParseInstructionError::BadValue)
    }

    fn parse_dir(s: &str) -> Result<isize, ParseInstructionError> {
        match s {
            "left" => Ok(-1),
            "right" => Ok(1),
            _ => Err(ParseInstructionError::BadValue),
        } 
    }

    fn swap_position(input: &str, a: usize, b: usize) -> String {
        let mut chars: Vec<char> = input.chars().collect();
        let temp = chars[a];
        chars[a] = chars[b];
        chars[b] = temp;
        return chars.iter().collect();
    }

    fn swap_letter(input: &str, a: char, b: char) -> String {
        let mut chars: Vec<char> = input.chars().collect();
        for i in 0..chars.len() {
            chars[i] = match chars[i] {
                x if x == a => b,
                x if x == b => a,
                x => x,
            };
        }
        return chars.iter().collect();
    }

    fn rotate_right(input: &str, rot: isize) -> String {
        let mut chars: Vec<char> = input.chars().collect();
        let count = chars.len() as isize;

        chars = (0..count)
            .map(|x| (x - rot + 2*count) % count)
            .map(|x| chars[x as usize])
            .collect();

        return chars.iter().collect();
    }

    fn rotate_on_letter(input: &str, letter: char) -> String {
        let pos = input
            .chars()
            .position(|x| x == letter)
            .expect("letter to rotate on not found")
            as isize;

        let rot = 1 + pos + if pos>=4 { 1 } else { 0 };

        return Self::rotate_right(input, rot);
    }

    fn reverse(input: &str, a: usize, b: usize) -> String {
        let mut chars: Vec<char> = input.chars().collect();
        let len = b - a + 1;
        for i in 0..(len/2) {
            let temp = chars[a + i];
            chars[a + i] = chars[b - i];
            chars[b - i] = temp;
        }
        return chars.iter().collect();
    }

    fn move_position(input: &str, a: usize, b: usize) -> String {
        let mut chars: Vec<char> = input.chars().collect();
        let temp = chars.remove(a);
        chars.insert(b, temp);
        return chars.iter().collect();
    }

    fn execute(&self, input: &str) -> String {
        match self {
            Instruction::SwapPosition(a, b) => Self::swap_position(input, *a, *b),
            Instruction::SwapLetter(a, b) => Self::swap_letter(input, *a, *b),
            Instruction::Reverse(a, b) => Self::reverse(input, *a, *b),
            Instruction::RotateRight(rot) => Self::rotate_right(input, *rot),
            Instruction::Move(a, b) => Self::move_position(input, *a, *b),
            Instruction::RotateOnLetter(letter) => Self::rotate_on_letter(input, *letter),
        }
    }

    fn execute_rev(&self, input: &str) -> String {
        match self {
            // same as execute
            Instruction::SwapPosition(a, b) => Self::swap_position(input, *a, *b),
            Instruction::SwapLetter(a, b) => Self::swap_letter(input, *a, *b),
            Instruction::Reverse(a, b) => Self::reverse(input, *a, *b),

            // slightly different
            Instruction::RotateRight(rot) => Self::rotate_right(input, -(*rot)),
            Instruction::Move(a, b) => Self::move_position(input, *b, *a),
            
            // don't know how to reverse: just brute force it
            Instruction::RotateOnLetter(letter) => {
                for i in 0..(input.len()+1) {
                    let maybe_orig = Self::rotate_right(input, -(i as isize));
                    if Self::rotate_on_letter(&maybe_orig, *letter) == input {
                        return maybe_orig;
                    }
                }
                panic!("failed to reverse RotateOnLetter")
            },
        }
    }
}

impl FromStr for Instruction {
    type Err = ParseInstructionError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {

        match s.split_whitespace().collect::<Vec<_>>().as_slice() {

            [ "swap", "position", a, "with", "position", b ]
                => Ok(Instruction::SwapPosition(Instruction::parse_value(a)?, Instruction::parse_value(b)?)),

            [ "swap", "letter", a, "with", "letter", b ]
                => Ok(Instruction::SwapLetter(Instruction::parse_value(a)?, Instruction::parse_value(b)?)),
            
            [ "rotate", dir, count, _ ]
                => Ok(Instruction::RotateRight(Instruction::parse_dir(dir)? * Instruction::parse_value::<isize>(count)?)),
            
            [ "rotate", "based", "on", "position", "of", "letter", letter ]
                => Ok(Instruction::RotateOnLetter(Instruction::parse_value(letter)?)),
            
            [ "reverse", "positions", a, "through", b ]
                => Ok(Instruction::Reverse(Instruction::parse_value(a)?, Instruction::parse_value(b)?)),
            
            [ "move", "position", a, "to", "position", b ]
                => Ok(Instruction::Move(Instruction::parse_value(a)?, Instruction::parse_value(b)?)),
            
            _ => Err(ParseInstructionError::InvalidInstruction(s.to_string())),
        }
    }
}

fn main() {
    let example_instructions = parse_instructions("../example").expect("can't read example file");
    let puzzle_instructions = parse_instructions("../input").expect("can't read input file");

    let example_start = "abcde";
    let part1_start = "abcdefgh";
    let part2_end = "fbgdceah";

    let example_scrambled = scramble(example_start, &example_instructions);
    let example_unscrambled = unscramble(&example_scrambled, &example_instructions);

    println!("Example input:       '{}'", example_start);
    println!("Example scrambled:   '{}'", example_scrambled);
    println!("Example unscrambled: '{}' {}", example_unscrambled,
             if example_start == example_unscrambled { "ok" } else { "error" });

    let part1_scrambled = scramble(part1_start, &puzzle_instructions);
    let part1_unscrambled = unscramble(&part1_scrambled, &puzzle_instructions);

    println!("Part 1 start:       '{}'", part1_start);
    println!("Part 1 scrambled:   '{}'", part1_scrambled);
    println!("Part 1 unscrambled: '{}' {}", part1_unscrambled,
             if part1_unscrambled == part1_start { "ok" } else { "error" });

    let part2_unscrambled = unscramble(part2_end, &puzzle_instructions);
    let part2_scrambled = scramble(&part2_unscrambled, &puzzle_instructions);

    println!("Part 2 end:         '{}'", part2_end);
    println!("Part 2 unscrambled: '{}'", part2_unscrambled);
    println!("Part 2 scrambled:   '{}' {}", part2_scrambled,
             if part2_scrambled == part2_end { "ok" } else { "error" });
}

fn parse_instructions(filename: &str) -> Result<Vec<Instruction>, ParseInstructionError> {
    std::fs::read_to_string(filename)
        .expect("can't read file")
        .lines()
        .map(|s| s.parse())
        .collect()
}

fn scramble(start_value: &str, instructions: &[Instruction]) -> String {
    let mut current_value: String = start_value.to_string();
    for instr in instructions {
        current_value = instr.execute(&current_value);
    }
    return current_value;
}

fn unscramble(end_value: &str, instructions: &[Instruction]) -> String {
    let mut current_value: String = end_value.to_string();
    for instr in instructions.iter().rev() {
        //print!("unscramble {} ... ", current_value);
        current_value = instr.execute_rev(&current_value);
        //println!("{} ({:?})", current_value, instr);
    }
    return current_value;
}

#[test]
fn test_instruction_execute_rotate() {
    fn test(start: &str, dir: isize, end: &str) {
        let instr = Instruction::RotateRight(dir);
        let scrambled = instr.execute(start);
        let unscrambled = instr.execute_rev(&scrambled);
        assert_eq!(end.to_string(), scrambled);
        assert_eq!(start.to_string(), unscrambled);
    }

    test("abc", -5, "cab");
    test("abc", -4, "bca");
    test("abc", -3, "abc");
    test("abc", -2, "cab");
    test("abc", -1, "bca");
    test("abc", 0, "abc");
    test("abc", 1, "cab");
    test("abc", 2, "bca");
    test("abc", 3, "abc");
    test("abc", 4, "cab");
    test("abc", 5, "bca");
    test("abc", 6, "abc");
}

#[test]
fn test_instruction_execute_rotate_on_letter() {
    fn test(start: &str, letter: char, end: &str) {
        let instr = Instruction::RotateOnLetter(letter);
        let scrambled = instr.execute(start);
        let unscrambled = instr.execute_rev(&scrambled);
        assert_eq!(end.to_string(), scrambled);
        assert_eq!(start.to_string(), unscrambled);
    }

    test("abc", 'a', "cab");
    test("abc", 'b', "bca");
    test("abc", 'c', "abc");

    test("abcdefgh", 'a', "habcdefg");
    test("abcdefgh", 'b', "ghabcdef");
    test("abcdefgh", 'c', "fghabcde");
    test("abcdefgh", 'd', "efghabcd");
    test("abcdefgh", 'e', "cdefghab");
    test("abcdefgh", 'f', "bcdefgha");
    test("abcdefgh", 'g', "abcdefgh");
    test("abcdefgh", 'h', "habcdefg");
}

