use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

fn main() {

    let start_time = Instant::now();

    if std::env::args().len() != 2 {
        eprintln!("Missing input file");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();
    let positions = parse_input(&filename);
    let num_elements = positions.len() as u32;

    let initial_state = convert_positions_to_state(&positions);
    println!("initial state: {:#b}", initial_state.value);
    initial_state.pretty_print(num_elements);

    let num_steps = find_minimum_steps_to_goal(initial_state, num_elements);
    println!("Minimum steps to solution: {num_steps}");

    let total_runtime = start_time.elapsed();

    println!("Total runtime: {:?}", total_runtime);
}

fn find_minimum_steps_to_goal(initial_state: State, num_elements: u32) -> u32 {

    let mut closed_list: HashSet<u32> = HashSet::new();
    let mut open_list: VecDeque<(State,u32)> = VecDeque::new();

    open_list.push_back((initial_state, 0));

    while open_list.len() > 0 {

        // Using VecDeque as a FIFO, resulting in a BFS.
        let (current_state, num_steps) = open_list.pop_front().unwrap();
        
        if current_state.is_goal(num_elements) {
            println!("reached goal:");
            current_state.pretty_print(num_elements);
            // Because we use BFS, this is the optimal solution.
            return num_steps;
        }

        for next_state in current_state.get_all_possible_next_states(num_elements) {
            if closed_list.contains(&next_state.value) { continue; }
            if !next_state.is_legal(num_elements) { continue; }

            closed_list.insert(next_state.value);
            open_list.push_back((next_state, num_steps+1));
        }
    }

    panic!("No solution was found.");
}

/// state:
/// - elevator position
/// - element1 generator position
/// - element1 microchip position
/// ...
/// - elementN generator position
/// - elementN microchip position
///
/// with position=0..3 (2bits)
struct State {
    value: u32,
}

impl State {
    fn new(elevator: u32, positions: &[(u32,u32)] ) -> State {
        State {
            value: Self::encode_value(elevator, positions),
        }
    }

    fn with_elevator(&self, elevator: u32) -> State {
        assert!(elevator <= 3);
        return State {
            value: self.value & !3 | elevator,
        };
    }

    fn with_gen(&self, element_id: u32, new_gen_pos: u32) -> State {
        let offset: u32 = 2 + 4 * element_id;
        assert!(offset <= 30);
        assert!(new_gen_pos <= 3);
        return State {
            value: self.value & !(3<<offset) | (new_gen_pos<<offset),
        };
    }

    fn with_chip(&self, element_id: u32, new_chip_pos: u32) -> State {
        let offset: u32 = 2 + 2 + 4 * element_id;
        assert!(offset <= 30);
        assert!(new_chip_pos <= 3);
        return State {
            value: self.value & !(3<<offset) | (new_chip_pos<<offset),
        };
    }

    fn encode_value(elevator: u32, positions: &[(u32,u32)]) -> u32 {
        assert!(2 + 4 * positions.len() <= 32, "Too many elements, can't encode state in 32 bits");
        let mut offset: u32 = 0;
        let mut value: u32 = 0;

        assert!(elevator < 4);
        value |= (elevator as u32) << offset;
        offset += 2;

        for pos in positions {
            assert!(pos.0 < 4);
            assert!(pos.1 < 4);
            value |= (pos.0 as u32) << offset;
            offset += 2;
            value |= (pos.1 as u32) << offset;
            offset += 2;
        }
        return value;
    }

    fn get_elevator_position(&self) -> u32 {
        self.value & 0x03
    }

    fn get_element_position(&self, element_id: u32) -> (u32, u32) {
        let mut temp = self.value >> (2 + element_id * 4);
        let a = temp & 0x03;
        temp = temp >> 2;
        let b = temp & 0x03;
        return (a,b);
    }

    #[allow(dead_code)] // Used by test
    fn get_all_element_positions(&self, num_elements: u32) -> Vec<(u32,u32)> {
        let mut positions = Vec::with_capacity(num_elements as usize);
        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            positions.push(p);
        }
        return positions;
    }

    fn pretty_print(&self, num_elements: u32) {
        let elev_pos = self.get_elevator_position();
        for floor in (0..4).rev() {
            let elev_str = if floor == elev_pos { "E" } else { "." };
            let mut pos_str: String = String::new();
            for elem in 0..num_elements {
                let p = self.get_element_position(elem);
                let gen_str: String = if p.0 == floor { format!("{elem}G ") } else { ".  ".to_string() };
                let chip_str: String = if p.1 == floor { format!("{elem}M ") } else { ".  ".to_string() };
                pos_str.push_str(&gen_str);
                pos_str.push_str(&chip_str);
            }
            println!("F{} | {} | {}", floor, elev_str, pos_str);
        }
    }

    fn get_all_possible_next_states(&self, num_elements: u32) -> Vec<State> {
        let num_floors = 4;
        let current_pos = self.get_elevator_position();
        
        let mut available_gens: Vec<u32> = Vec::with_capacity(num_elements as usize);
        let mut available_chips: Vec<u32> = Vec::with_capacity(num_elements as usize);

        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            if p.0 == current_pos {
                available_gens.push(elem);
            }
            if p.1 == current_pos {
                available_chips.push(elem);
            }
        }

        let mut available_next_states: Vec<State> = Vec::with_capacity(10);

        let can_go_up = current_pos < num_floors-1;
        let can_go_down = current_pos > 0;

        // Case 1: Take one gen
        for gen in &available_gens {
            if can_go_up {
                available_next_states.push(self
                    .with_elevator(current_pos + 1)
                    .with_gen(*gen, current_pos + 1));
            }
            if can_go_down {
                 available_next_states.push(self
                    .with_elevator(current_pos - 1)
                    .with_gen(*gen, current_pos - 1));
            }
        }

        // Case 2: Take on chip
        for chip in &available_chips {
            if can_go_up {
                available_next_states.push(self
                                           .with_elevator(current_pos + 1)
                                           .with_chip(*chip, current_pos + 1));
            }
            if can_go_down {
                 available_next_states.push(self
                                            .with_elevator(current_pos - 1)
                                            .with_chip(*chip, current_pos - 1));
            }
        }

        // Case 3: Take two gens
        if available_gens.len() > 1 {
            for gen1 in &available_gens {
                for gen2 in &available_gens {
                    if *gen1 >= *gen2 { continue; }

                    if can_go_up {
                        let up_pos = current_pos + 1;
                        available_next_states.push(self
                                                   .with_elevator(up_pos)
                                                   .with_gen(*gen1, up_pos)
                                                   .with_gen(*gen2, up_pos)
                                                   );
                    }
                    if can_go_down {
                        let down_pos = current_pos - 1;
                        available_next_states.push(self
                                                   .with_elevator(down_pos)
                                                   .with_gen(*gen1, down_pos)
                                                   .with_gen(*gen2, down_pos)
                                                   );
                    }
                }
            }
        }

        // Case 4: Take two chips
        if available_chips.len() > 1 {
            for chip1 in &available_chips {
                for chip2 in &available_chips {
                    if *chip1 >= *chip2 { continue; }

                    if can_go_up {
                        let up_pos = current_pos + 1;
                        available_next_states.push(self
                                                   .with_elevator(up_pos)
                                                   .with_chip(*chip1, up_pos)
                                                   .with_chip(*chip2, up_pos)
                                                   );
                    }
                    if can_go_down {
                        let down_pos = current_pos - 1;
                        available_next_states.push(self
                                                   .with_elevator(down_pos)
                                                   .with_chip(*chip1, down_pos)
                                                   .with_chip(*chip2, down_pos)
                                                   );
                    }
                }
            }
        }

        // Case 5: Take gen + chip
        for gen in &available_gens {
            for chip in &available_chips {

                if can_go_up {
                    let up_pos = current_pos + 1;
                    available_next_states.push(self
                                               .with_elevator(up_pos)
                                               .with_gen(*gen, up_pos)
                                               .with_chip(*chip, up_pos)
                                              );
                }
                if can_go_down {
                    let down_pos = current_pos - 1;
                    available_next_states.push(self
                                               .with_elevator(down_pos)
                                               .with_gen(*gen, down_pos)
                                               .with_chip(*chip, down_pos)
                                              );
                }
            }
        }

        return available_next_states;
    }

    fn is_legal(&self, num_elements: u32) -> bool {
        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            let is_shielded = p.0 == p.1;

            let mut is_radiated: bool = false;
            for other_elem in 0..num_elements {
                if elem == other_elem { continue; }
                if self.get_element_position(other_elem).0 == p.1 {
                    is_radiated = true;
                }
            }

            // Fried chip?
            if is_radiated && !is_shielded {
                return false;
            }
        }

        return true;
    }

    fn is_goal(&self, num_elements: u32) -> bool {
        let num_floors = 4;
        let goal_floor = num_floors - 1;

        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            if p.0 != goal_floor || p.1 != goal_floor {
                return false;
            }
        }

        return true;
    }
}

#[test]
fn test_state_encoding()
{
    for i in 0..=3 {
        let s = State::new(i, &[]);
        assert_eq!(i, s.get_elevator_position());
    }

    for i in 0..=3 {
        let s = State::new(i, &[ (0,1), (2,3) ]);
        assert_eq!(i, s.get_elevator_position());
        assert_eq!((0,1), s.get_element_position(0));
        assert_eq!((2,3), s.get_element_position(1));
    }
}

#[test]
fn test_state_with_methods()
{
    let s = State::new(0, &[ (0,0), (0,0), (0,0) ]);
    assert_eq!(vec![ (0,0), (0,0), (0,0) ], s.get_all_element_positions(3));
    
    assert_eq!(vec![ (1,0), (0,0), (0,0) ], s.with_gen(0,1).get_all_element_positions(3));
    assert_eq!(vec![ (0,0), (2,0), (0,0) ], s.with_gen(1,2).get_all_element_positions(3));
    assert_eq!(vec![ (0,0), (0,0), (3,0) ], s.with_gen(2,3).get_all_element_positions(3));
    assert_eq!(vec![ (0,1), (0,0), (0,0) ], s.with_chip(0,1).get_all_element_positions(3));
    assert_eq!(vec![ (0,0), (0,2), (0,0) ], s.with_chip(1,2).get_all_element_positions(3));
    assert_eq!(vec![ (0,0), (0,0), (0,3) ], s.with_chip(2,3).get_all_element_positions(3));

    assert_eq!(0, s.get_elevator_position());
    assert_eq!(0, s.with_elevator(0).get_elevator_position());
    assert_eq!(3, s.with_elevator(3).get_elevator_position());

    assert_eq!(
        vec![ (2,3), (1,2), (3,1) ],
        s
        .with_chip(2,1)
        .with_chip(1,2)
        .with_chip(0,3)
        .with_elevator(3)
        .with_gen(0,2)
        .with_gen(1,1)
        .with_gen(2,0)
        .with_gen(2,3)
        .get_all_element_positions(3));
}

fn parse_input(filename: &String) -> HashMap<u32, (u32, u32)> {
    let content = std::fs::read_to_string(filename);
    if let Err(err) = content {
        eprintln!("Error: {err}");
        panic!();
    }

    // element_name -> element_id (determines position in state-bitmap)
    let mut element_ids: HashMap<String, u32> = HashMap::new();
    let mut get_element_id = |element_name: String| -> u32 {
        if let Some(id) = element_ids.get(&element_name) {
            return *id;
        }
        let next_id = element_ids.len() as u32;
        element_ids.insert(element_name, next_id);
        return next_id;
    };

    // element_id -> (generator_pos, chip_pos)
    let mut positions: HashMap<u32, (u32, u32)> = HashMap::new();
    let mut set_position_of_x = |element_id: u32, floor: u32, is_chip: bool| {
        let mut val: (u32,u32) = *positions.get(&element_id).unwrap_or( &(0,0) );
        if is_chip {
            val.1 = floor;
        } else {
            val.0 = floor;
        }
        positions.insert(element_id, val);
    };

    for line in content.unwrap().lines() {

        let split_chars: &[_] = &[' '];
        let trim_chars: &[_] = &['.', ','];
        let filter_words = ["a", "an", "and", "nothing", "relevant"];

        let parts: Vec<&str> = line
            .split(split_chars)
            .map(|s| s.trim_matches(trim_chars))
            .filter(|s| !filter_words.contains(s))
            .map(|s| sanitize_element_name(s))
            .collect();

        match parts.as_slice() {
            ["The", floor_name, "floor", "contains", ..] => {
                let floor = parse_floor_name(floor_name);
                let remain = &parts[4..];
                assert!(remain.len() % 2 == 0); // Must be multiple of 2

                for pair in remain.chunks(2) {
                    match pair {
                        [element, kind] => {
                            let element_id = get_element_id(element.to_string());
                            let is_chip = *kind == "microchip";
                            set_position_of_x(element_id, floor, is_chip);
                        },
                        _ => panic!("Can't match pair"),
                    }
                }
            },
            _ => panic!("Invalid input"),
        }
    }

    println!("Elements: {element_ids:?}");
    return positions;
}

fn sanitize_element_name(name: &str) -> &str {
    let bad_word = "-compatible";
    if name.ends_with(bad_word) {
        &name[0.. name.len() - bad_word.len()]
    } else {
        name
    }
}

fn parse_floor_name(name: &str) -> u32 {
    match name {
        "first" => 0,
        "second" => 1,
        "third" => 2,
        "fourth" => 3,
        _ => panic!("Invalid floor name"),
    }
}

fn convert_positions_to_state(positions: &HashMap<u32, (u32, u32)>) -> State {
    let mut positions_vector: Vec<(u32, u32)> = Vec::new();

    for i in 0..positions.len() {
        let p = *positions.get(&(i as u32)).unwrap();
        positions_vector.push(p);
    }

    return State::new(0, &positions_vector );
}
