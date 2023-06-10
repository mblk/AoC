use std::collections::HashMap;

#[derive(Debug, Copy, Clone)]
enum Target {
    Bot(usize),
    Output(usize),
}

impl Target {
    fn new(kind: &str, id: &str) -> Target {
        let id_val: usize = id.parse().unwrap();
        match kind {
            "bot" => Target::Bot(id_val),
            "output" => Target::Output(id_val),
            _ => panic!("invalid target kind"),
        }
    }
}

#[derive(Debug, Copy, Clone)]
enum Instruction {
    Const {
        value: u32,
        target: Target,
    },
    Compare {
        id: usize,
        low_target: Target,
        high_target: Target,
    },
}

struct State {
    bot_data: Vec<(Option<u32>, Option<u32>)>,
    output_data: Vec<Option<u32>>,
}

impl State {
    fn new(num_bots: usize, num_outputs: usize) -> State {
        State {
            bot_data: vec![(None,None); num_bots],
            output_data: vec![None; num_outputs],
        }
    }

    fn set_value(&mut self, target: &Target, value: u32) {
        match target {
            Target::Bot(bot_id) => {
                if self.bot_data[*bot_id].0 == None {
                    self.bot_data[*bot_id].0 = Some(value);
                } else if self.bot_data[*bot_id].1 == None {
                    self.bot_data[*bot_id].1 = Some(value);
                } else {
                    panic!("bot already has 2 values");
                }
            },
            Target::Output(output_id) => {
                assert_eq!(None, self.output_data[*output_id]);
                self.output_data[*output_id] = Some(value);
            },
        }
    }
}

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file name");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();
    let (instructions, num_bots, num_outputs) = parse_instructions(&filename);
    let sorted_instructions = sort_instructions(&instructions);
    let mut state = State::new(num_bots, num_outputs);

    for instr in &sorted_instructions {
        match instr {
            Instruction::Const { target, value } => {
                state.set_value(target, *value);
            },
            Instruction::Compare { id, low_target, high_target } => {
                let val1 = state.bot_data[*id].0.unwrap();
                let val2 = state.bot_data[*id].1.unwrap();

                let low_val  = if val1 < val2 { val1 } else { val2 };
                let high_val = if val1 < val2 { val2 } else { val1 };

                if val1 == 61 && val2 == 17 {
                    println!("bot {id} is comparing {val1} to {val2}");
                }

                state.set_value(low_target, low_val);
                state.set_value(high_target, high_val);
            },
        }
    }

    assert!(state.output_data.len() >= 3);

    let product_of_first_three: u32 = state.output_data
        .iter()
        .take(3)
        .map(|x| x.unwrap())
        .product();

    println!("product of first 3 outputs: {product_of_first_three}");
}

fn instr_is_ready(instr: &Instruction, num_values_for_id: &HashMap<usize, usize>) -> bool {
    match instr {
        // const is always ready as it has no inputs
        Instruction::Const { value: _, target: _ } => true,

        // compare is ready if two input values are available
        Instruction::Compare { id, low_target: _, high_target: _ } =>
            match num_values_for_id.get(id) {
                Some(cnt) => *cnt == 2,
                None => false,
            },
    }
}

fn increase_num_values(num_values_for_id: &mut HashMap<usize, usize>, target: &Target) {
    if let Target::Bot(id) = target {
        num_values_for_id.entry(*id)
            .and_modify(|v| *v += 1)
            .or_insert(1);
    }
}

fn update_num_values(instr: &Instruction, num_values_for_id: &mut HashMap<usize, usize>) {
    match instr {
        Instruction::Const { value: _, target } => {
            increase_num_values(num_values_for_id, target);
        },
        Instruction::Compare { id: _, low_target, high_target } => {
            increase_num_values(num_values_for_id, low_target);
            increase_num_values(num_values_for_id, high_target);
        },
    }
}

fn sort_instructions(instructions: &[Instruction]) -> Vec<Instruction> {
    let mut result = Vec::new();
    let mut num_values_for_id: HashMap<usize, usize> = HashMap::new();

    let mut temp_instructions: Vec<Instruction> = instructions.to_vec();

    while temp_instructions.len() > 0 {

        let ready_instr = temp_instructions
            .iter()
            .position(|instr| instr_is_ready(instr, &num_values_for_id));

        match ready_instr {
            Some(instr_idx) => {
                let instr = temp_instructions.remove(instr_idx);
                result.push(instr);
                update_num_values(&instr, &mut num_values_for_id);
            },
            None => panic!("stuck"),
        }
    }

    return result;
}

fn parse_instructions(filename: &String) -> (Vec<Instruction>, usize, usize) {
    let mut instructions = Vec::new();
    
    let mut max_bot_id: usize = 0;
    let mut max_output_id: usize = 0;

    let mut update_max_ids = |target: &Target| {
        match target {
            Target::Bot(bot_id) => if *bot_id > max_bot_id { max_bot_id = *bot_id },
            Target::Output(output_id) => if *output_id > max_output_id { max_output_id = *output_id },
        }
    };

    for line in std::fs::read_to_string(filename).unwrap().lines() {
        let parts: Vec<&str> = line.split(" ").collect();
        match parts.as_slice() {
            ["value", value_str, "goes", "to", target_type, target_id] => {
                let value: u32 = value_str.parse().unwrap();
                let target = Target::new(target_type, target_id);
                update_max_ids(&target);
                instructions.push(Instruction::Const {
                    value, target,
                });
            },
            ["bot", id_str, "gives", "low", "to", low_type, low_id, "and", "high", "to", high_type, high_id] => {
                let id: usize = id_str.parse().unwrap();
                let low_target = Target::new(low_type, low_id);
                let high_target = Target::new(high_type, high_id);
                update_max_ids(&low_target);
                update_max_ids(&high_target);
                instructions.push(Instruction::Compare {
                    id, low_target, high_target,
                });
            },
            _ => panic!("Can't parse line"),
        }
    }

    let num_bots = max_bot_id + 1;
    let num_outputs = max_output_id + 1;

    return (instructions, num_bots, num_outputs);
}

