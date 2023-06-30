fn main() {
    println!("Example: {}", solve_puzzle("10000", 20));
    println!("Part 1:  {}", solve_puzzle("01111010110010011", 272));
    println!("Part 2:  {}", solve_puzzle("01111010110010011", 35651584));
}

fn solve_puzzle(input: &str, target_length: usize) -> String {
    let seq = string_to_seq(input);
    let data = extend_seq(&seq, target_length);
    let checksum = calc_checksum_for_seq(&data);
    return seq_to_string(&checksum);
}

fn string_to_seq(input: &str) -> Vec<u8> {
    input
        .chars()
        .map(|c| if c == '1' { 1 } else { 0 })
        .collect()
}

fn seq_to_string(seq: &[u8]) -> String {
    seq.iter()
        .map(|b| if *b==1 { '1'} else { '0' })
        .collect()
}

fn extend_seq(input: &[u8], target_length: usize) -> Vec<u8> {
    let mut data: Vec<u8> = Vec::with_capacity(target_length);
    data.extend_from_slice(input);

    while data.len() < target_length {
        let seq: Vec<u8> = data.iter()
            .rev()
            .map(|b| (*b+1)%2)
            .collect();
        data.extend([0]);
        data.extend(seq);
    }

    return data[0..target_length].to_vec();
}

#[test]
fn test_extend_seq() {
    assert_eq!(vec![1,0,0], extend_seq(&[1], 3));
    assert_eq!(vec![1,0,0,0,0,0,1,1,1,1,0,0,1,0,0,0,0,1,1,1], extend_seq(&[1,0,0,0,0], 20));
}

fn calc_checksum_for_seq(input: &[u8]) -> Vec<u8> {
    let mut checksum: Vec<u8> = input.to_vec();
    while checksum.len() % 2 == 0 {
        checksum = checksum.chunks(2)
            .map(|x| if x[0]==x[1] { 1 } else { 0 })
            .collect();
    }
    return checksum;
}
