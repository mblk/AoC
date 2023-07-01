use std::collections::VecDeque;
use md5;

#[derive(Debug, Copy, Clone, Eq, PartialEq)]
struct Position(u8, u8);

#[derive(Debug, Clone)]
struct Node {
    pos: Position,
    path: String,
}

#[derive(Debug, Clone, Copy)]
enum Direction {
    Left, Right, Up, Down,
}

fn main() {
    println!("Shortest path:");
    println!("Example 1: {}", find_shortest_path_to_vault("ihgpwlah"));
    println!("Example 2: {}", find_shortest_path_to_vault("kglvqrro"));
    println!("Example 3: {}", find_shortest_path_to_vault("ulqzkmiv"));
    println!("My input : {}", find_shortest_path_to_vault("bwnlcvfs"));

    println!("Longest path:");
    println!("Example 1: {}", find_longest_path_to_vault("ihgpwlah"));
    println!("Example 2: {}", find_longest_path_to_vault("kglvqrro"));
    println!("Example 3: {}", find_longest_path_to_vault("ulqzkmiv"));
    println!("My input : {}", find_longest_path_to_vault("bwnlcvfs"));
}

fn find_longest_path_to_vault(passcode: &str) -> usize {
    let mut open_list: VecDeque<Node> = VecDeque::new();

    open_list.push_back(Node {
        pos: Position(0, 0),
        path: "".to_string(),
    });

    let mut longest_path_length: usize = 0;

    while !open_list.is_empty() {
        let current_node = open_list.pop_front().unwrap();
        if current_node.pos == Position(3,3) {
            if current_node.path.len() > longest_path_length {
                longest_path_length = current_node.path.len();
            }
        } else {
            let next_nodes = get_possible_nodes(&current_node, passcode);
            for next in next_nodes {
                open_list.push_back(next);
            }
        }
    }

    return longest_path_length;
}


fn find_shortest_path_to_vault(passcode: &str) -> String {
    let mut open_list: VecDeque<Node> = VecDeque::new();

    open_list.push_back(Node {
        pos: Position(0, 0),
        path: "".to_string(),
    });

    while !open_list.is_empty() {
        let current_node = open_list.pop_front().unwrap();
        if current_node.pos == Position(3,3) {
            return current_node.path;
        }

        let next_nodes = get_possible_nodes(&current_node, passcode);
        for next in next_nodes {
            open_list.push_back(next);
        }
    }

    panic!("no path found");
}

fn get_possible_nodes(current_node: &Node, passcode: &str) -> Vec<Node> {
    let mut nodes: Vec<Node> = Vec::new();
    let pos = current_node.pos;
    let path = &current_node.path;

    if pos.0 > 0 && door_is_open(path, Direction::Left, passcode) {
        nodes.push(Node {
            pos: Position(pos.0 - 1, pos.1),
            path: path.to_string() + "L",
        });
    }
    if pos.0 < 3 && door_is_open(path, Direction::Right, passcode) {
        nodes.push(Node {
            pos: Position(pos.0 + 1, pos.1),
            path: path.to_string() + "R",
        });
    }
    if pos.1 > 0 && door_is_open(path, Direction::Up, passcode) {
        nodes.push(Node {
            pos: Position(pos.0, pos.1 - 1),
            path: path.to_string() + "U",
        });
    }
    if pos.1 < 3 && door_is_open(path, Direction::Down, passcode) {
        nodes.push(Node {
            pos: Position(pos.0, pos.1 + 1),
            path: path.to_string() + "D",
        });
    }

    return nodes;
}

fn door_is_open(path: &str, direction: Direction, passcode: &str) -> bool {
    let value_to_hash = format!("{passcode}{path}");
    let h = format!("{:x}", md5::compute(value_to_hash));

    let c = match direction {
        Direction::Up => h.chars().nth(0),
        Direction::Down => h.chars().nth(1),
        Direction::Left => h.chars().nth(2),
        Direction::Right => h.chars().nth(3),
    }.unwrap();

    return is_door_open_char(c);
}

fn is_door_open_char(c: char) -> bool {
    c >= 'b' && c <= 'f'
}
