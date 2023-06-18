use md5;
use std::time::Instant;

fn main() -> Result<(), ()> {
    let args: Vec<String> = std::env::args().collect();

    if args.len() != 3 {
        eprintln!("Invalid args");
        return Err(());
    }

    let salt: String = args[1].clone();
    let stretch: bool = args[2].parse().unwrap();

    let result = calc_one_time_pad(&salt, stretch);
    println!("Found one time pad at index: {result}");

    return Ok(());
}

#[test]
fn test_example_no_stretch() {
    assert_eq!(22728, calc_one_time_pad("abc", false));
}

#[test]
fn test_example_with_stretch() {
    assert_eq!(22551, calc_one_time_pad("abc", true));
}

#[test]
fn test_input_no_stretch() {
    assert_eq!(35186, calc_one_time_pad("jlmsuwbz", false));
}

#[test]
fn test_input_with_stretch() {
    assert_eq!(22429, calc_one_time_pad("jlmsuwbz", true));
}

fn calc_one_time_pad(salt: &str, stretch: bool) -> usize {
    let start_time = Instant::now();
    let window_size: usize = 1000;

    // Pre-calculate the next x hashes and store them in a ring-buffer.
    let mut hashes_ringbuf: Vec<String> = Vec::with_capacity(window_size);
    let mut hashes_cur_pos: usize = 0;
    for i in 0..window_size {
        hashes_ringbuf.push(calculate_hash_for_index(salt, stretch, i));
    }

    let mut criteria_met_counter: usize = 0;
    let mut current_index: usize = 0;
    loop {
        let current_hash = hashes_ringbuf[hashes_cur_pos].clone();
        hashes_ringbuf[hashes_cur_pos] = calculate_hash_for_index(salt, stretch, current_index + window_size);
        hashes_cur_pos = (hashes_cur_pos + 1) % window_size;

        if let Some(triplet) = has_triplet(&current_hash) {
            if hashes_ringbuf.iter().any(|h| has_sequence(h, triplet)) {
                criteria_met_counter += 1;
                if criteria_met_counter == 64 {
                    let dur = start_time.elapsed();
                    println!("Runtime: {:?}", dur);
                    return current_index;
                }
            }
        }
        current_index += 1;
    }
}

fn calculate_hash_for_index(salt: &str, stretch: bool, index: usize) -> String {
    let mut hash = hash_string(&format!("{salt}{index}"));
    if stretch {
        for _ in 0..2016 {
            hash = hash_string(&hash);
        }
    }
    return hash;
}

fn hash_string(input: &String) -> String {
    format!("{:x?}", md5::compute(input))
}

fn has_triplet(input: &str) -> Option<u8> {
    let chars = input.as_bytes();
    for i in 0..input.len()-2 {
        if chars[i] == chars[i+1] && chars[i] == chars[i+2] {
            return Some(chars[i]);
        }
    }
    return None;
}

fn has_sequence(input: &str, seq: u8) -> bool {
    let chars = input.as_bytes();
    for i in 0..input.len()-4 {
        if (&chars[i..i+5]).iter().all(|x| *x == seq) {
            return true;
        }
    }
    return false;
}

