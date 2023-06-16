use std::collections::{HashSet, HashMap, BinaryHeap, VecDeque};
use std::cmp::Ordering;

#[derive(Debug, Clone, Copy, Eq, PartialEq, Hash)]
struct Position {
    x: u32,
    y: u32,
}

impl Position {
    fn new(x: u32, y: u32) -> Position {
        Position { x, y }
    }

    fn distance(&self, other: &Position) -> f64 {
        let dx = other.x as f64 - self.x as f64;
        let dy = other.y as f64 - self.y as f64;
        return (dx*dx + dy*dy).sqrt();
    }
}

impl std::fmt::Display for Position {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "({},{})", self.x, self.y)
    }
}

struct PositionWithScore {
    pos: Position,
    score: f64,
}

impl PositionWithScore {
    fn new(pos: Position, score: f64) -> PositionWithScore {
        PositionWithScore { pos, score }
    }
}

impl PartialEq for PositionWithScore {
    fn eq(&self, other: &Self) -> bool {
        self.score == other.score
    }
}

impl Eq for PositionWithScore {
    // empty?
}

impl PartialOrd for PositionWithScore {
    fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

impl Ord for PositionWithScore {
    fn cmp(&self, other: &Self) -> Ordering {
        // We want to use this type to create a min-heap, so this is reversed:
        if self.score < other.score {
            Ordering::Greater
        } else if self.score > other.score {
            Ordering::Less
        } else {
            Ordering::Equal
        }
    }
}

fn main() -> Result<(), ()> {
    let args: Vec<String> = std::env::args().collect();
    if args.len() != 7 {
        eprintln!("Invalid args");
        return Err(());
    }
    let map_seed: u32 = args[1].parse().unwrap();
    let start_x: u32 = args[2].parse().unwrap();
    let start_y: u32 = args[3].parse().unwrap();
    let target_x: u32 = args[4].parse().unwrap();
    let target_y: u32 = args[5].parse().unwrap();
    let max_steps: u32 = args[6].parse().unwrap();
    
    let start = Position::new(start_x, start_y);
    let target = Position::new(target_x, target_y);
    let map_width: u32 = std::cmp::max(start.x, target.x) + 3;
    let map_height: u32 = std::cmp::max(start.y, target.y) + 3;

    println!("Map:");
    draw_map(map_seed, map_width, map_height);

    let path = find_shortest_path(map_seed, start, target).unwrap();
    println!("\nMin steps to target: {}", path.len()-1);
    draw_map_with_path(map_seed, map_width, map_height, path.as_slice());

    let reachable_in_x_steps = flood_fill(map_seed, start, max_steps);
    println!("\nReachable in {max_steps} steps: {} locations", reachable_in_x_steps.len());
    draw_map_with_path(map_seed, map_width, map_height, reachable_in_x_steps.as_slice());

    return Ok(());
}

#[test]
fn test_shortest_path()
{
    assert_eq!(11 + 1, find_shortest_path(10, Position::new(1,1), Position::new(7,4)).expect("bubu").len());
    assert_eq!(86 + 1, find_shortest_path(1364, Position::new(1,1), Position::new(31,39)).expect("bubu").len());
}

#[test]
fn test_flood_fill()
{
    assert_eq!(11, flood_fill(10, Position::new(1,1), 5).len());
    assert_eq!(127, flood_fill(1364, Position::new(1,1), 50).len());
}

#[test]
fn test_position_with_cost()
{
    let mut h: BinaryHeap<PositionWithScore> = BinaryHeap::new();
    h.push(PositionWithScore::new(Position::new(1, 1), 1.3));
    h.push(PositionWithScore::new(Position::new(3, 3), 1.1));
    h.push(PositionWithScore::new(Position::new(2, 2), 1.2));

    // Should pop the lowest cost first
    assert_eq!(Position::new(3, 3), h.pop().unwrap().pos);
    assert_eq!(Position::new(2, 2), h.pop().unwrap().pos);
    assert_eq!(Position::new(1, 1), h.pop().unwrap().pos);
}

fn flood_fill(seed: u32, start: Position, max_depth: u32) -> Vec<Position> {
    let mut open_list: VecDeque<(Position,u32)> = VecDeque::new();
    let mut closed_list: HashSet<Position> = HashSet::new();

    open_list.push_back((start, 0));

    while !open_list.is_empty() {

        let (current_node, current_depth) = open_list.pop_front().unwrap();

        for neighbour in get_neighbours(current_node) {
            if is_wall(seed, neighbour.x, neighbour.y) { continue; }
            if closed_list.contains(&neighbour) { continue; }

            if current_depth + 1 < max_depth {
                open_list.push_back((neighbour, current_depth + 1));
            }
            closed_list.insert(neighbour);
        }
    }

    return closed_list.iter().map(|p| *p).collect();
}

fn find_shortest_path(seed: u32, start: Position, target: Position) -> Option<Vec<Position>> {
    // Based on wiki/A*
    let mut open_set: BinaryHeap<PositionWithScore> = BinaryHeap::new();
    let mut g_scores: HashMap<Position, f64> = HashMap::new();
    let mut came_from: HashMap<Position, Position> = HashMap::new();

    open_set.push(PositionWithScore::new(start, 0.0));
    g_scores.insert(start, 0.0);

    while !open_set.is_empty() {
        // Get node with lowest f_score (=g_score + heuristic cost to target)
        let current: PositionWithScore = open_set.pop().unwrap();
        
        if current.pos == target {
            return Some(reconstruct_path(&came_from, current.pos));
        }

        for neighbour in get_neighbours(current.pos) {
           
            // get best g-score for neighbour so far
            let neighbour_g_score: f64 = *g_scores.get(&neighbour).unwrap_or(&f64::MAX);
        
            // calculate potential new g-score 
            let tentative_g_score: f64 = *g_scores.get(&current.pos).unwrap() +
                get_step_cost(seed, current.pos, neighbour);
            
            // found better solution?
            if tentative_g_score < neighbour_g_score {

                g_scores.insert(neighbour, tentative_g_score);

                // add to open-set with new f-score (g_score + heuristic cost to target)
                let new_f_score = tentative_g_score + get_heuristic_cost(neighbour, target);
                open_set.push(PositionWithScore::new(neighbour, new_f_score));
                
                came_from.insert(neighbour, current.pos);
            }
        }
    }

    return None;
}

fn reconstruct_path(came_from: &HashMap<Position, Position>, end_position: Position) -> Vec<Position> {
    let mut path: Vec<Position> = Vec::new();
    let mut current_position: Position = end_position;

    loop {
        path.push(current_position);

        let previous = came_from.get(&current_position);
        if let Some(previous_position) = previous {
            current_position = *previous_position;
        } else {
            break;
        }
    }

    return path;
}

fn get_step_cost(seed: u32, current: Position, next: Position) -> f64 {
    let mut cost: f64 = current.distance(&next);
    if is_wall(seed, next.x, next.y) {
        cost += 1000.0;
    }
    if is_wall(seed, current.x, current.y) {
        cost += 1000.0;
    }
    return cost;
}

fn get_neighbours(node: Position) -> Vec<Position> {
    let mut neighbours = Vec::new();

    // left
    if node.x > 0 {
        neighbours.push(Position::new(node.x - 1, node.y));
    }
    // up
    if node.y > 0 {
        neighbours.push(Position::new(node.x, node.y - 1));
    }
    // right
    neighbours.push(Position::new(node.x + 1, node.y));
    // down
    neighbours.push(Position::new(node.x, node.y + 1));

    return neighbours;
}

fn get_heuristic_cost(start: Position, target: Position) -> f64 {
    return start.distance(&target);
}

fn draw_map(seed: u32, width: u32, height: u32) {
    for y in 0..height {
        for x in 0..width {
            if is_wall(seed, x, y) {
                print!("#");
            } else {
                print!(" ");
            }
        }
        println!("");
    }
}

fn draw_map_with_path(seed: u32, width: u32, height: u32, path: &[Position]) {
    for y in 0..height {
        for x in 0..width {
            if is_wall(seed, x, y) {
                print!("#");
            } else {
                let path_index = path.iter().position(|p| p.x == x && p.y == y);
                if let Some(_) = path_index {
                    print!(".");
                } else {
                    print!(" ");
                }
            }
        }
        println!("");
    }
}

fn is_wall(seed: u32, x: u32, y: u32) -> bool {
    let temp = x*x + 3*x + 2*x*y + y + y*y + seed;
    let ones = temp.count_ones();
    return ones % 2 != 0;
}
