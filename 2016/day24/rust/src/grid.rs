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
        &self.data[index.1*self.width + index.0]
    }
}

impl<T> IndexMut<(usize, usize)> for Grid<T> {
    fn index_mut(&mut self, index: (usize, usize)) -> &mut Self::Output {
        &mut self.data[index.1*self.width + index.0]
    }
}

