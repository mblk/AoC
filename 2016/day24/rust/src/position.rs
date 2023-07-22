use std::cmp::Ordering;

#[derive(Debug, Clone, Copy, Eq, PartialEq, Hash)]
pub struct Position {
    pub x: usize,
    pub y: usize,
}

pub struct PositionWithScore {
    pub pos: Position,
    pub score: f64,
}

impl Position {
    pub fn new(x: usize, y: usize) -> Position {
        Position { x, y }
    }

    pub fn distance(&self, other: &Position) -> f64 {
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

impl PositionWithScore {
    pub fn new(pos: Position, score: f64) -> PositionWithScore {
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

