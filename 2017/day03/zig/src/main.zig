const std = @import("std");

const Direction = enum {
    Right,
    Up,
    Left,
    Down,

    fn next(self: Direction) Direction {
        return switch (self) {
            Direction.Right => Direction.Up,
            Direction.Up => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Down => Direction.Right,
        };
    }
};

const Coord = struct {
    x: i32,
    y: i32,

    fn new(x: i32, y: i32) Coord {
        return Coord{
            .x = x,
            .y = y,
        };
    }

    fn with_offset(self: Coord, dx: i32, dy: i32) Coord {
        return Coord{
            .x = self.x + dx,
            .y = self.y + dy,
        };
    }

    fn move(self: *Coord, dir: Direction) void {
        switch (dir) {
            Direction.Right => self.x += 1,
            Direction.Up => self.y += 1,
            Direction.Left => self.x -= 1,
            Direction.Down => self.y -= 1,
        }
    }

    fn get_distance_to_center(self: Coord) u32 {
        const x_abs = std.math.absInt(self.x) catch unreachable;
        const y_abs = std.math.absInt(self.y) catch unreachable;
        return @as(u32, @intCast(x_abs + y_abs));
    }
};

fn get_distance_to_value(target_value: u32) u32 {
    const c = get_value_coords(target_value);
    const d = c.get_distance_to_center();
    return d;
}

fn get_value_coords(target_value: u32) Coord {
    var pos = Coord.new(0, 0);
    var dir = Direction.Right;
    var steps: u32 = 1;
    var value: u32 = 1;

    while (true) {
        for (0..steps) |_| {
            if (value == target_value)
                return pos;

            pos.move(dir);
            value += 1;
        }

        dir = dir.next();

        if (dir == Direction.Left or dir == Direction.Right)
            steps += 1;
    }
}

fn stress_test(target_value: u32) !u32 {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer {
        const r = gpa.deinit();
        std.debug.assert(r == std.heap.Check.ok);
    }
    const allocator = gpa.allocator();

    var map = std.AutoHashMap(Coord, u32).init(allocator);
    defer map.deinit();
    try map.put(Coord.new(0, 0), 1);

    var pos = Coord.new(0, 0);
    var dir = Direction.Right;
    var steps: u32 = 1;

    while (true) {
        for (0..steps) |_| {
            var value: u32 = 0;
            var dy: i32 = -1;
            while (dy <= 1) : (dy += 1) {
                var dx: i32 = -1;
                while (dx <= 1) : (dx += 1) {
                    const adj_pos = pos.with_offset(dx, dy);
                    if (map.get(adj_pos)) |adj_val| value += adj_val;
                }
            }

            if (value > target_value)
                return value;

            try map.put(pos, value);

            pos.move(dir);
        }

        dir = dir.next();

        if (dir == Direction.Left or dir == Direction.Right)
            steps += 1;
    }
}

pub fn main() !void {
    const input: usize = 347991;
    std.log.info("Part1: {any}", .{get_distance_to_value(input)});
    std.log.info("Part2: {any}", .{stress_test(input)});
}

test "test examples for part1" {
    try std.testing.expectEqual(@as(u32, 0), get_distance_to_value(1));
    try std.testing.expectEqual(@as(u32, 3), get_distance_to_value(12));
    try std.testing.expectEqual(@as(u32, 2), get_distance_to_value(23));
    try std.testing.expectEqual(@as(u32, 31), get_distance_to_value(1024));
}

test "test examples for part2" {
    try std.testing.expectEqual(@as(u32, 147), try stress_test(142));
    try std.testing.expectEqual(@as(u32, 806), try stress_test(800));
}
