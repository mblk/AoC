use std::collections::{HashMap, BinaryHeap};

use super::node_grid::{NodeState, NodeGrid};
use super::position::{Position, PositionWithScore};

pub fn find_shortest_path(grid: &NodeGrid, blacklist: &[Position], start: Position, target: Position)
    -> Option<Vec<Position>> {

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

        for neighbour in get_neighbours(grid, current.pos) {
          
            if is_wall(grid, neighbour) || blacklist.contains(&neighbour) {
                continue;
            }

            // get best g-score for neighbour so far
            let neighbour_g_score: f64 = *g_scores.get(&neighbour).unwrap_or(&f64::MAX);
        
            // calculate potential new g-score 
            let tentative_g_score: f64 = *g_scores.get(&current.pos).unwrap() +
                get_heuristic_cost(current.pos, neighbour);
            
            // found better solution?
            if tentative_g_score < neighbour_g_score {

                // add to open-set with new f-score (g_score + heuristic cost to target)
                let new_f_score = tentative_g_score + get_heuristic_cost(neighbour, target);
                open_set.push(PositionWithScore::new(neighbour, new_f_score));
                
                g_scores.insert(neighbour, tentative_g_score);
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

    path.reverse();

    return path;
}

fn is_wall(grid: &NodeGrid, pos: Position) -> bool {
    match grid[(pos.x, pos.y)] {
        NodeState::Full => true,
        _ => false,
    }
}

fn get_heuristic_cost(start: Position, target: Position) -> f64 {
    return start.distance(&target);
}

fn get_neighbours(grid: &NodeGrid, node: Position) -> Vec<Position> {
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
    if node.x < grid.width-1 {
        neighbours.push(Position::new(node.x + 1, node.y));
    }
    // down
    if node.y < grid.height-1 {
        neighbours.push(Position::new(node.x, node.y + 1));
    }

    return neighbours;
}

#[allow(dead_code)]
pub fn draw_map(grid: &NodeGrid) {
    println!("map:");
    for y in 0..grid.height {
        for x in 0..grid.width {
            if is_wall(grid, Position::new(x, y)) {
                print!("#");
            } else {
                print!(".");
            }
        }
        println!("");
    }
}

#[allow(dead_code)]
pub fn draw_map_with_path(grid: &NodeGrid, path: &[Position]) {
    println!("map with path:");
    for y in 0..grid.height {
        for x in 0..grid.width {
            if is_wall(grid, Position::new(x, y)) {
                print!("#");
            } else {
                let path_index = path.iter().position(|p| p.x == x && p.y == y);
                if let Some(_) = path_index {
                    print!("x");
                } else {
                    print!(".");
                }
            }
        }
        println!("");
    }
}

