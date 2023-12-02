use std::fs::read_to_string;

fn main() {
    let input = std::env::args()
        .nth(1)
        .expect("missing input file");

    let lines: Vec<String> = read_to_string(input)
        .unwrap()
        .lines()
        .map(String::from)
        .collect();

    let part1: u32 = lines
        .iter()
        .map(|s| parse_line1(s))
        .sum();

    let part2: u32 = lines
        .iter()
        .map(|s| parse_line2(s))
        .sum();

    println!("Part1: {}", part1);
    println!("Part2: {}", part2);
}

fn parse_line1(input: &str) -> u32 {
    let mut first: u32 = 0;
    let mut last: u32 = 0;

    for c in input.chars() {
        if let Some(value) = c.to_digit(10) {
            if first == 0 {
                first = value;
            }
            last = value;
        }
    }

    return first * 10 + last;
}

fn parse_line2(input: &str) -> u32 {
    let mut first: u32 = 0;
    let mut last: u32 = 0;
    let mut remaining: &str = input;

    while remaining.len() > 0 {
        if let Some(value) = parse_leading_digit(remaining) {
            if first == 0 {
                first = value;
            }
            last = value;
        }
        remaining = &remaining[1..];
    }

    return first * 10 + last;
}

fn parse_leading_digit(input: &str) -> Option<u32> {
    
    let first = input.chars()
        .nth(0)
        .unwrap();

    if let Some(value) = first.to_digit(10) {
        return Some(value);
    }
    return parse_spelled_out_digit(input);
}

fn parse_spelled_out_digit(input: &str) -> Option<u32> {
    return match input {
        s if s.starts_with("one") => Some(1),
        s if s.starts_with("two") => Some(2),
        s if s.starts_with("three") => Some(3),
        s if s.starts_with("four") => Some(4),
        s if s.starts_with("five") => Some(5),
        s if s.starts_with("six") => Some(6),
        s if s.starts_with("seven") => Some(7),
        s if s.starts_with("eight") => Some(8),
        s if s.starts_with("nine") => Some(9),
        _ => None,
    };
}
