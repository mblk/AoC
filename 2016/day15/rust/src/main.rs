use std::str::FromStr;

#[derive(Debug, Clone, Copy)]
struct Disc {
    id: u32,
    size: u32,
    start: u32,
}

impl Disc {
    fn new(id: u32, size: u32, start: u32) -> Disc {
        Disc { id, size, start }
    }
}

#[derive(Debug)]
struct DiscParseError;

impl FromStr for Disc {
    type Err = DiscParseError;

    fn from_str(input: &str) -> Result<Self, Self::Err> {

        let split_chars: &[_] = &[' '];
        let trim_chars: &[_] = &['#', ';', ',', '.'];

        let parts: Vec<&str> = input
            .split(split_chars)
            .map(|s| s.trim_matches(trim_chars))
            .collect();

        match parts.as_slice() {
            [ "Disc", id, "has", size, "positions",
                "at", "time=0", "it", "is", "at", "position", start ] => {

                let id_val: u32 = id.parse().map_err(|_| DiscParseError)?;
                let size_val: u32 = size.parse().map_err(|_| DiscParseError)?;
                let start_val: u32 = start.parse().map_err(|_| DiscParseError)?;

                Ok(Disc::new(id_val, size_val, start_val))
            }
            _ => Err(DiscParseError)
        }
    }
}

fn main() -> Result<(),String> {
    let args: Vec<String> = std::env::args().collect();
    if args.len() != 2 {
        return Err("Missing input file".to_string());
    }

    let filename: &str = args.iter().nth(1).unwrap();
    let mut discs1 = parse_discs(filename)?;
    discs1.sort_by(|a,b| a.id.cmp(&b.id));

    let mut discs2 = discs1.clone();
    discs2.push(Disc::new(99, 11, 0));

    println!("Part1: {}", get_capsule(&discs1));
    println!("Part2: {}", get_capsule(&discs2));

    return Ok(());
}

fn parse_discs(filename: &str) -> Result<Vec<Disc>, String> {
    let data: String = std::fs::read_to_string(filename)
        .map_err(|e| e.to_string())?;
    
    let discs: Result<Vec<Disc>, _> = data
        .lines()
        .map(|s| s.parse().map_err(|_| "Parse error".to_string()))
        .collect();

    return discs;
}

fn get_capsule(discs: &[Disc]) -> u32 {
    for push_time in 0.. {
        let got_capsule = push_button(&discs, push_time);
        if got_capsule {
            return push_time;
        }
    }
    panic!();
}

fn push_button(discs: &[Disc], push_time: u32) -> bool {
    let mut t: u32 = push_time + 1;
    for d in discs {
        let aligned: bool = (d.start + t) % d.size == 0;
        if !aligned {
            return false;
        }       
        t += 1;
    }
    return true;
}
