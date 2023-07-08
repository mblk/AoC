fn main() {
    let args: Vec<String> = std::env::args().collect();
    if args.len() != 2 { eprintln!("Missing input file"); return; }

    let rules = parse_rules(&args[1]);

    println!("Part 1: {}", find_next_free_port(&rules, 0).unwrap());
    println!("Part 2: {}", get_free_port_count(&rules));
}

fn find_next_free_port(rules: &[(u32, u32)], starting_port: u32) -> Option<u32> {
    let mut current_port: u32 = starting_port;
    loop {
        match rules.iter().find(|r| r.0 <= current_port && current_port <= r.1) {
            Some(next_rule) => {
                if next_rule.1 == 0xFFFF_FFFF { return None; }
                current_port = next_rule.1 + 1;
            },
            None => return Some(current_port),
        };
    }
}

fn get_free_port_count(rules: &[(u32, u32)]) -> u32 {
    let mut free_ports: u32 = 0;
    let mut current_port: u32 = 0;
    loop {
        match find_next_free_port(rules, current_port) {
            Some(p) => current_port = p,
            None => break,
        };
        match rules.iter().find(|r| r.0 >= current_port) {
            Some(next_rule) => {
                free_ports += next_rule.0 - current_port;
                current_port = next_rule.0;
            },
            None => break,
        };
    }
    return free_ports;
}

fn parse_rules(filename: &str) -> Vec<(u32, u32)> {
    let mut rules: Vec<(u32, u32)> = std::fs::read_to_string(filename)
        .expect("failed to read file")
        .lines()
        .map(|s| {
            let (a, b) = s.split_once('-').expect("missing delim");
            let min: u32 = a.parse().expect("not a u32");
            let max: u32 = b.parse().expect("not a u32");
            return (min, max);
        })
        .collect();

    rules.sort_by(|a,b| a.0.cmp(&b.0));

    return rules;
}
