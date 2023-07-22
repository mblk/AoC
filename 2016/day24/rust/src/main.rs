mod grid;
mod map;
mod pathing;
mod position;

use map::{Locations, Map};
use std::collections::{HashMap, VecDeque};

fn main() {
    let filename = std::env::args().nth(1).expect("missing argument");

    let (map, locations) = Map::load(&filename);
    let costs = calculate_costs(&map, &locations);

    //pathing::draw_map(&map);
    //println!("Locations: {:#?}", locations);
    //println!("Costs: {:#?}", costs);

    let location_ids: Vec<usize> = locations.keys().map(|id| id.to_owned()).collect();

    println!("Part1: {}", find_best_route(&location_ids, &costs, false));
    println!("Part2: {}", find_best_route(&location_ids, &costs, true));
}

fn find_best_route(
    locations: &[usize],
    costs: &HashMap<(usize, usize), usize>,
    return_home: bool,
) -> usize {

    struct SearchNode {
        route: Vec<usize>,
        cost: usize,
    }
    let mut open_list: VecDeque<SearchNode> = VecDeque::new();
    let mut min_cost: usize = usize::MAX;

    open_list.push_back(SearchNode {
        route: vec![0],
        cost: 0,
    }); // id, cost

    while !open_list.is_empty() {
        let current_node = open_list.pop_front().unwrap();
        let current_location_id = current_node.route.last().unwrap().to_owned();

        for next_location_id in 0..locations.len() {
            if current_node.route.contains(&next_location_id) {
                continue;
            }

            let mut next_route = current_node.route.clone();
            next_route.push(next_location_id);

            let mut next_cost =
                current_node.cost + costs.get(&(current_location_id, next_location_id)).unwrap();

            // reached end?
            if next_route.len() == locations.len() {
                if return_home {
                    next_cost += costs.get(&(next_location_id, 0)).unwrap();
                }
                if next_cost < min_cost {
                    min_cost = next_cost;
                }
            } else {
                open_list.push_back(SearchNode {
                    route: next_route,
                    cost: next_cost,
                });
            }
        }
    }

    return min_cost;
}

fn calculate_costs(map: &Map, locations: &Locations) -> HashMap<(usize, usize), usize> {
    let location_ids: Vec<usize> = locations.keys().map(|id| *id).collect();
    let mut costs: HashMap<(usize, usize), usize> = HashMap::new();

    for loc1_id in &location_ids {
        for loc2_id in &location_ids {
            let loc1_pos = locations.get(loc1_id).unwrap().to_owned();
            let loc2_pos = locations.get(loc2_id).unwrap().to_owned();

            let path = pathing::find_shortest_path(&map, &[], loc1_pos, loc2_pos).expect("no path");

            costs.insert((*loc1_id, *loc2_id), path.len() - 1);
            costs.insert((*loc2_id, *loc1_id), path.len() - 1);
        }
    }

    return costs;
}
