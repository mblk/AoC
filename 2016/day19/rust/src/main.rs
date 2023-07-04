fn main() {
    println!("Example 1: {}", solve_puzzle_1(5));
    println!("Example 2: {}", solve_puzzle_2(5));
    println!("Part 1:    {}", solve_puzzle_1(3014387));
    println!("Part 2:    {}", solve_puzzle_2(3014387));
}

fn solve_puzzle_2(count: usize) -> usize {
    let mut presents: Vec<u8> = Vec::with_capacity(count);
    for _ in 0..count { presents.push(1); }

    let mut active_index: usize = 0;
    let mut target_index: usize = count / 2;
    let mut move_target_twice: bool = true;
    let mut remaining: usize = count;

    loop {
        if active_index == target_index {
            assert_eq!(1, remaining);
            return active_index + 1;
        }

        assert!(presents[active_index] == 1);
        assert!(presents[target_index] == 1);
        presents[target_index] = 0;
        remaining -= 1;

        loop {
            active_index = (active_index + 1) % count;
            if presents[active_index] == 1 { break; }
        }
        loop {
            target_index = (target_index + 1) % count;
            if presents[target_index] == 1 { break; }
        }
        if move_target_twice {
            loop {
                target_index = (target_index + 1) % count;
                if presents[target_index] == 1 { break; }
            }
        }
        move_target_twice = !move_target_twice;
    }
}

fn solve_puzzle_1(count: usize) -> usize {
    let mut presents: Vec<u8> = Vec::with_capacity(count);
    for _ in 0..count { presents.push(1); }

    let mut active_index: usize = 0;
    let mut target_index: usize = 1;
    let mut remaining: usize = count;

    loop {
        if active_index == target_index {
            assert_eq!(1, remaining);
            return active_index + 1;
        }

        assert!(presents[active_index] == 1);
        assert!(presents[target_index] == 1);
        presents[target_index] = 0;
        remaining -= 1;

        loop {
            active_index = (active_index + 1) % count;
            if presents[active_index] == 1 { break; }
        }
        loop {
            target_index = (target_index + 1) % count;
            if presents[target_index] == 1 { break; }
        }
        loop {
            target_index = (target_index + 1) % count;
            if presents[target_index] == 1 { break; }
        }
    }
}
