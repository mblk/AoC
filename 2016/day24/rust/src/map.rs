use std::collections::HashMap;

use super::grid::Grid;
use super::position::Position;

pub type Map = Grid<bool>;
pub type Locations = HashMap<usize, Position>;

impl Map {
    pub fn load(filename: &str) -> (Map, Locations) {
        let data = std::fs::read_to_string(filename).unwrap();
        let width = data.lines().map(|s| s.len()).max().unwrap();
        let height = data.lines().count();

        let mut map = Map::new(width, height);
        let mut locations: HashMap<usize, Position> = HashMap::new();

        for (y, line) in data.lines().enumerate() {
            for (x, c) in line.chars().enumerate() {
                map[(x, y)] = match c {
                    '#' => true,
                    _ => false,
                };

                if char::is_digit(c, 10) {
                    let id = c.to_string().parse::<usize>().unwrap();
                    let pos = Position::new(x, y);
                    locations.insert(id, pos);
                }
            }
        }

        return (map, locations);
    }
}
