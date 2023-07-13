mod disk_usage;
mod position;
mod grid;
mod node_grid;
mod pathing;

use disk_usage::DiskUsage;
use node_grid::NodeGrid;
use position::Position;
use pathing::find_shortest_path;

fn main() {
    let filename = std::env::args().nth(1).expect("missing argument");
    let usages = DiskUsage::parse_file(&filename);

    println!("Viable pairs: {}", count_viable_pairs(&usages));
    println!("Total steps: {}", move_data_to_access_port(&usages).expect("no solution"));
}

fn count_viable_pairs(usages: &[DiskUsage]) -> usize {
    let mut viable_pairs: usize = 0;
    for u1 in usages {
        for u2 in usages {
            if u1.used > 0 && u1.path != u2.path && u1.used <= u2.avail {
                viable_pairs += 1;
            }
        }
    }
    return viable_pairs;
}

fn move_data_to_access_port(usages: &[DiskUsage]) -> Option<usize> {
    let grid = NodeGrid::create(&usages);

    let empty_spot = grid.find_empty_spot();
    let access_position = Position::new(0, 0);
    let data_position = Position::new(grid.width-1, 0);
    
    let data_path = find_shortest_path(&grid, &[], data_position, access_position)?;

    let mut current_data_position = data_position;
    let mut current_empty_position = empty_spot;
    let mut total_steps: usize = 0;

    for target_pos in data_path {
        if current_data_position == target_pos { continue; }

        // move empty spot to target
        let path = find_shortest_path(&grid, &[current_data_position], current_empty_position, target_pos)?;
        current_empty_position = target_pos;
        total_steps += path.len() - 1;

        // swap empty spot and data spot
        (current_empty_position, current_data_position) = (current_data_position, current_empty_position);
        total_steps += 1;
    }

    return Some(total_steps);
}

