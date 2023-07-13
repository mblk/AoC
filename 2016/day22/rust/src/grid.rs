use std::ops::{Index, IndexMut};

#[derive(Debug)]
pub struct Grid<T> {
    pub width: usize,
    pub height: usize,
    data: Vec<T>,
}

impl<T: Default + Clone> Grid<T> {
    pub fn new(width: usize, height: usize) -> Self {
        Self {
            width,
            height,
            data: vec!(T::default(); width * height),
        }
    }
}

impl<T> Index<(usize, usize)> for Grid<T> {
    type Output = T;

    fn index(&self, index: (usize, usize)) -> &Self::Output {
        assert!(index.0 < self.width, "x-index out of bounds");
        assert!(index.1 < self.height, "y-indesx out of bounds");
        &self.data[index.1*self.width + index.0]
    }
}

impl<T> IndexMut<(usize, usize)> for Grid<T> {
    fn index_mut(&mut self, index: (usize, usize)) -> &mut Self::Output {
        assert!(index.0 < self.width, "x-index out of bounds");
        assert!(index.1 < self.height, "y-indesx out of bounds");
        &mut self.data[index.1*self.width + index.0]
    }
}

impl<T: std::fmt::Display> std::fmt::Display for Grid<T> {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        writeln!(f, "Grid:")?;
        for y in 0..self.height {
            for x in 0..self.width {
                let value = &self[(x, y)];
                write!(f, "{} ", value)?;
            }
            writeln!(f, "")?;
        }
        return Ok(());
    }
}

