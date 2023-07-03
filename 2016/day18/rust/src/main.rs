fn main() {
    let input = ".^.^..^......^^^^^...^^^...^...^....^^.^...^.^^^^....^...^^.^^^...^^^^.^^.^.^^..^.^^^..^^^^^^.^^^..^";

    println!("Example 1: {}", count_safe_cells("..^^.", 3));
    println!("Example 2: {}", count_safe_cells(".^^.^.^^^^", 10));
    println!("Part 1: {}", count_safe_cells(input, 40));
    println!("Part 2: {}", count_safe_cells(input, 400000));
}

fn count_safe_cells(first_row_str: &str, total_rows: usize) -> usize {
    let mut current_row: Vec<bool> = first_row_str
        .chars()
        .map(|c| c == '^')
        .collect();

    let mut trap_count: usize = 0;

    for _ in 0..total_rows {
        trap_count += current_row.iter()
            .filter(|b| **b==false)
            .count();
        current_row = create_next_row(&current_row);
    }

    return trap_count;
}

fn create_next_row(current_row: &[bool]) -> Vec<bool> {
    let len = current_row.len();
    let mut next_row: Vec<bool> = Vec::with_capacity(len);

    for i in 0..len {
        let x: [bool; 3] = [
            if i>0 { current_row[i-1] } else { false },
            current_row[i],
            if i<len-1 { current_row[i+1]} else { false },
        ];
        next_row.push(tile_is_trap(x));
    }

    return next_row;
}

fn tile_is_trap(input: [bool; 3]) -> bool {
    match input {
        [ true, true, false ] => true,
        [ false, true, true ] => true,
        [ true, false, false ] => true,
        [ false, false, true ] => true,
        _ => false,
    }
}
