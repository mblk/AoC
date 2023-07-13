use super::grid::Grid;
use super::disk_usage::DiskUsage;
use super::position::Position;

#[derive(Debug, Default, Clone, PartialEq)]
pub enum NodeState {
    #[default] Empty,
    Full,
    Movable,
}

pub type NodeGrid = Grid<NodeState>;

impl NodeGrid {
    pub fn create(usages: &[DiskUsage]) -> NodeGrid {
        let x_min = usages.iter().min_by_key(|x| x.px).unwrap().px;
        let x_max = usages.iter().max_by_key(|x| x.px).unwrap().px;
        let y_min = usages.iter().min_by_key(|x| x.py).unwrap().py;
        let y_max = usages.iter().max_by_key(|x| x.py).unwrap().py;
        let width = x_max - x_min + 1;
        let height = y_max - y_min + 1;
        //println!("Grid: {},{} ... {},{} ({},{})", x_min, y_min, x_max, y_max, width, height);

        let min_size = usages.iter().min_by_key(|x| x.size).unwrap().size;
        //println!("min_size={min_size}");

        let mut grid = NodeGrid::new(width, height);

        for usage in usages {
            grid[(usage.px, usage.py)] = match usage {
                x if x.used == 0 => NodeState::Empty,
                x if x.used <= min_size => NodeState::Movable,
                _ => NodeState::Full,
            };
        }
        //println!("{}", grid);

        return grid;
    }

    pub fn find_empty_spot(&self) -> Position {
        for y in 0..self.height {
            for x in 0..self.width {
                if self[(x,y)] == NodeState::Empty {
                    return Position::new(x,y);
                }
            }
        }
        panic!("can't find empty spot");
    }
}

impl std::fmt::Display for NodeState {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let c = match self {
            NodeState::Full => '#',
            NodeState::Empty => '_',
            NodeState::Movable => '.',
        };
        write!(f, "{}", c)
    }
}


