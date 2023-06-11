use std::collections::{HashMap, HashSet, VecDeque};

fn main() {
    if std::env::args().len() != 2 {
        eprintln!("Missing input file");
        return;
    }

    let filename = std::env::args().nth(1).unwrap();
    let positions = parse_input(&filename);
    let num_elements = positions.len() as u8;

    let initial_state = convert_positions_to_state(&positions);
    println!("initial state: {:#b}", initial_state.value);
    initial_state.pretty_print(num_elements);

    let num_steps = find_minimum_steps_to_goal(initial_state, num_elements);
    println!("Minimum steps to solution: {num_steps}");
}

fn find_minimum_steps_to_goal(initial_state: State, num_elements: u8) -> u32 {

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
    fn new(elevator: u8, positions: &[(u8,u8)] ) -> State {
        State {
            value: Self::encode_value(elevator, positions),
        }
    }

    fn encode_value(elevator: u8, positions: &[(u8,u8)]) -> u32 {
        assert!(2 + 2 * positions.len() <= 32, "Too many elements, can't encode state in 32 bits");
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

    fn get_elevator_position(&self) -> u8 {
        (self.value & 0x03) as u8
    }

    fn get_element_position(&self, element_id: u8) -> (u8, u8) {
        let mut temp = self.value >> (2 + element_id * 4);
        let a = (temp & 0x03) as u8;
        temp = temp >> 2;
        let b = (temp & 0x03) as u8;
        return (a,b);
    }

    fn get_all_element_positions(&self, num_elements: u8) -> Vec<(u8,u8)> {
        let mut positions = Vec::new();
        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            positions.push(p);
        }
        return positions;
    }

    fn pretty_print(&self, num_elements: u8) {
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

    fn get_all_possible_next_states(&self, num_elements: u8) -> Vec<State> {
        let num_floors = 4;
        let elev_pos = self.get_elevator_position();
        
        let mut available_gens: Vec<u8> = Vec::with_capacity(num_elements as usize);
        let mut available_chips: Vec<u8> = Vec::with_capacity(num_elements as usize);

        for elem in 0..num_elements {
            let p = self.get_element_position(elem);
            if p.0 == elev_pos {
                available_gens.push(elem);
            }
            if p.1 == elev_pos {
                available_chips.push(elem);
            }
        }

        let mut available_next_states: Vec<State> = Vec::new();

        let can_go_up = elev_pos < num_floors-1;
        let can_go_down = elev_pos > 0;

        // Case 1: Take one gen
        for gen in &available_gens {
            let mut positions = self.get_all_element_positions(num_elements);
            if can_go_up {
                let pos_up = elev_pos + 1;
                positions[*gen as usize].0 = pos_up;
                available_next_states.push(State::new(pos_up, &positions));
            }
            if can_go_down {
                let pos_down = elev_pos - 1;
                positions[*gen as usize].0 = pos_down;
                available_next_states.push(State::new(pos_down, &positions));
            }
        }

        // Case 2: Take on chip
        for chip in &available_chips {
            let mut positions = self.get_all_element_positions(num_elements);
            if can_go_up {
                let pos_up = elev_pos + 1;
                positions[*chip as usize].1 = pos_up;
                available_next_states.push(State::new(pos_up, &positions));
            }
            if can_go_down {
                let pos_down = elev_pos - 1;
                positions[*chip as usize].1 = pos_down;
                available_next_states.push(State::new(pos_down, &positions));
            }
        }

        // Case 3: Take two gens
        if available_gens.len() > 1 {
            for gen1 in &available_gens {
                for gen2 in &available_gens {
                    if *gen1 >= *gen2 { continue; }

                    let mut positions = self.get_all_element_positions(num_elements);
                    if can_go_up {
                        let pos_up = elev_pos + 1;
                        positions[*gen1 as usize].0 = pos_up;
                        positions[*gen2 as usize].0 = pos_up;
                        available_next_states.push(State::new(pos_up, &positions));
                    }
                    if can_go_down {
                        let pos_down = elev_pos - 1;
                        positions[*gen1 as usize].0 = pos_down;
                        positions[*gen2 as usize].0 = pos_down;
                        available_next_states.push(State::new(pos_down, &positions));
                    }
                }
            }
        }

        // Case 4: Take two chips
        if available_chips.len() > 1 {
            for chip1 in &available_chips {
                for chip2 in &available_chips {
                    if *chip1 >= *chip2 { continue; }

                    let mut positions = self.get_all_element_positions(num_elements);
                    if can_go_up {
                        let pos_up = elev_pos + 1;
                        positions[*chip1 as usize].1 = pos_up;
                        positions[*chip2 as usize].1 = pos_up;
                        available_next_states.push(State::new(pos_up, &positions));
                    }
                    if can_go_down {
                        let pos_down = elev_pos - 1;
                        positions[*chip1 as usize].1 = pos_down;
                        positions[*chip2 as usize].1 = pos_down;
                        available_next_states.push(State::new(pos_down, &positions));
                    }
                }
            }
        }

        // Case 5: Take gen + chip
        for gen in &available_gens {
            for chip in &available_chips {
                let mut positions = self.get_all_element_positions(num_elements);
                if can_go_up {
                    let pos_up = elev_pos + 1;
                    positions[*gen as usize].0 = pos_up;
                    positions[*chip as usize].1 = pos_up;
                    available_next_states.push(State::new(pos_up, &positions));
                }
                if can_go_down {
                    let pos_down = elev_pos - 1;
                    positions[*gen as usize].0 = pos_down;
                    positions[*chip as usize].1 = pos_down;
                    available_next_states.push(State::new(pos_down, &positions));
                }
            }
        }

        return available_next_states;
    }

    fn is_legal(&self, num_elements: u8) -> bool {
        let positions = self.get_all_element_positions(num_elements);
        for elem in 0..num_elements {
            let is_shielded = positions[elem as usize].0 == positions[elem as usize].1;
            let mut is_radiated: bool = false;

            for other_elem in 0..num_elements {
                if elem == other_elem { continue; }
                if positions[other_elem as usize].0 == positions[elem as usize].1 {
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

    fn is_goal(&self, num_elements: u8) -> bool {
        let num_floors = 4;
        let goal_floor = num_floors - 1;

        // All the things at the top floor?
        return self.get_all_element_positions(num_elements)
            .iter()
            .all(|t| t.0 == goal_floor && t.1 == goal_floor);
    }
}

#[test]
fn test_state_encoding()
{
    for i in 0..=3 {
        let floor = i as u8;
        let s = State::new(floor, &[]);
        assert_eq!(floor, s.get_elevator_position());
    }

    for i in 0..=3 {
        let floor = i as u8;
        let s = State::new(floor, &[ (0,1), (2,3) ]);
        assert_eq!(floor, s.get_elevator_position());
        assert_eq!((0,1), s.get_element_position(0));
        assert_eq!((2,3), s.get_element_position(1));
    }
}

fn parse_input(filename: &String) -> HashMap<u8, (u8, u8)> {
    let content = std::fs::read_to_string(filename);
    if let Err(err) = content {
        eprintln!("Error: {err}");
        panic!();
    }

    // element_name -> element_id (determines position in state-bitmap)
    let mut element_ids: HashMap<String, u8> = HashMap::new();
    let mut get_element_id = |element_name: String| -> u8 {
        if let Some(id) = element_ids.get(&element_name) {
            return *id;
        }
        let next_id = element_ids.len() as u8;
        element_ids.insert(element_name, next_id);
        return next_id;
    };

    // element_id -> (generator_pos, chip_pos)
    let mut positions: HashMap<u8, (u8, u8)> = HashMap::new();
    let mut set_position_of_x = |element_id: u8, floor: u8, is_chip: bool| {
        let mut val: (u8,u8) = *positions.get(&element_id).unwrap_or( &(0,0) );
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

fn parse_floor_name(name: &str) -> u8 {
    match name {
        "first" => 0,
        "second" => 1,
        "third" => 2,
        "fourth" => 3,
        _ => panic!("Invalid floor name"),
    }
}

fn convert_positions_to_state(positions: &HashMap<u8, (u8, u8)>) -> State {
    let mut positions_vector: Vec<(u8, u8)> = Vec::new();

    for i in 0..positions.len() {
        let p = *positions.get(&(i as u8)).unwrap();
        positions_vector.push(p);
    }

    return State::new(0, &positions_vector );
}
