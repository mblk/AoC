const std = @import("std");

// zig build-exe -O ReleaseFast main.zig && ./main ../input

pub const std_options = std.Options{
    .log_level = .info,
};

const Position = struct {
    x: i32,
    y: i32,

    fn createFrom(comptime T: type, x: T, y: T) Position {
        return Position{
            .x = @intCast(x),
            .y = @intCast(y),
        };
    }

    fn move(self: Position, dir: Direction) Position {
        const delta = dir.getDelta();

        return Position{
            .x = self.x + delta[0],
            .y = self.y + delta[1],
        };
    }

    fn equals(self: Position, other: Position) bool {
        return self.x == other.x and self.y == other.y;
    }
};

const Direction = enum {
    up,
    down,
    left,
    right,

    fn getDelta(self: Direction) [2]i32 {
        return switch (self) {
            .up => [2]i32{ 0, -1 },
            .down => [2]i32{ 0, 1 },
            .left => [2]i32{ -1, 0 },
            .right => [2]i32{ 1, 0 },
        };
    }

    fn turnRight(self: Direction) Direction {
        return switch (self) {
            .up => .right,
            .right => .down,
            .down => .left,
            .left => .up,
        };
    }
};

const Guard = struct {
    pos: Position,
    dir: Direction,

    fn walk(self: *Guard, map: *const Map) void {
        const p_next = self.pos.move(self.dir);

        if (!map.isValidPosition(p_next)) {
            self.pos = p_next;
        } else if (map.get(p_next) == 0) {
            self.pos = p_next;
        } else {
            self.dir = self.dir.turnRight();
        }
    }

    pub fn format(
        self: Guard,
        comptime fmt: []const u8,
        options: std.fmt.FormatOptions,
        writer: anytype,
    ) !void {
        _ = fmt;
        _ = options;

        try writer.print("Guard at ({d},{d}) looking ", .{ self.pos.x, self.pos.y });

        if (std.enums.tagName(Direction, self.dir)) |dir_name| {
            try writer.print("{s}", .{dir_name});
        } else {
            try writer.print("{any}", .{self.dir});
        }
    }
};

const Map = struct {
    width: usize,
    height: usize,
    data: []u8,
    start: Position,

    fn create(allocator: std.mem.Allocator, width: usize, height: usize) !Map {
        const data = try allocator.alloc(u8, width * height);
        @memset(data, 0);

        return Map{
            .width = width,
            .height = height,
            .data = data,
            .start = Position.createFrom(u32, 0, 0),
        };
    }

    fn set(self: *Map, pos: Position, data: u8) void {
        std.debug.assert(self.isValidPosition(pos));

        const x: usize = @intCast(pos.x);
        const y: usize = @intCast(pos.y);
        const index = y * self.width + x;

        self.data[index] = data;
    }

    fn get(self: *const Map, pos: Position) u8 {
        std.debug.assert(self.isValidPosition(pos));

        const x: usize = @intCast(pos.x);
        const y: usize = @intCast(pos.y);
        const index: usize = y * self.width + x;

        return self.data[index];
    }

    fn isValidPosition(self: *const Map, pos: Position) bool {
        if (pos.x < 0 or pos.y < 0 or pos.x >= self.width or pos.y >= self.height) {
            return false;
        }
        return true;
    }
};

pub fn main() !void {
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    const alloc = arena.allocator();

    const args = try std.process.argsAlloc(alloc);
    if (args.len != 2) {
        std.log.err("missing filename", .{});
        return;
    }

    var map = try parseMap(args[1], alloc);

    const part1_positions = try solvePart1(&map, alloc);
    std.log.info("part1: {d}", .{part1_positions.len});
    std.log.info("part2: {d}", .{try solvePart2(&map, part1_positions, alloc)});
}

fn solvePart1(map: *const Map, allocator: std.mem.Allocator) ![]Position {
    var guard = Guard{
        .pos = map.start,
        .dir = .up,
    };

    var visited_positions = std.AutoHashMap(Position, u8).init(allocator);

    while (map.isValidPosition(guard.pos)) {
        try visited_positions.put(guard.pos, 1);
        guard.walk(map);
    }

    // convert hash map to array and return it for part2
    var visited_positions_array = try allocator.alloc(Position, visited_positions.count());
    var iter = visited_positions.iterator();
    var i: usize = 0;

    while (iter.next()) |entry| {
        visited_positions_array[i] = entry.key_ptr.*;
        i += 1;
    }
    std.debug.assert(i == visited_positions.count());
    std.debug.assert(i == visited_positions_array.len);

    return visited_positions_array;
}

fn solvePart2(map: *Map, part1_positions: []Position, allocator: std.mem.Allocator) !usize {
    var total_loop_positions: usize = 0;

    for (part1_positions) |potential_position| {
        if (potential_position.equals(map.start)) continue;

        // block
        std.debug.assert(map.get(potential_position) == 0);
        map.set(potential_position, 2);

        // check
        if (try checkForLoop(map, allocator)) {
            total_loop_positions += 1;
        }

        // unblock
        map.set(potential_position, 0);
    }

    return total_loop_positions;
}

fn checkForLoop(map: *const Map, allocator: std.mem.Allocator) !bool {
    var guard = Guard{
        .pos = map.start,
        .dir = .up,
    };
    var prev_pos = guard.pos;
    var visited_positions = std.AutoHashMap(Guard, u8).init(allocator);

    while (true) {
        try visited_positions.put(guard, 1);

        guard.walk(map);

        if (!guard.pos.equals(prev_pos)) {
            if (visited_positions.get(guard)) |_| {
                return true;
            }
        }
        prev_pos = guard.pos;

        if (!map.isValidPosition(guard.pos)) {
            return false;
        }
    }
}

fn parseMap(filename: []const u8, allocator: std.mem.Allocator) !Map {
    const file_data = try std.fs.cwd().readFileAlloc(allocator, filename, 1024 * 1024);

    var lines = std.mem.splitScalar(u8, file_data, '\n');

    var map_width: usize = 0;
    var map_height: usize = 0;

    while (lines.next()) |line| {
        map_width = @max(map_width, line.len);
        if (line.len > 0) {
            map_height += 1;
        }
    }

    //std.log.info("map: w={d} h={d}", .{ map_width, map_height });

    lines.reset();

    var map = try Map.create(allocator, map_width, map_height);

    var y: usize = 0;
    while (lines.next()) |line| {
        var x: usize = 0;
        for (line) |c| {
            const pos = Position.createFrom(usize, x, y);
            if (c == '#') {
                map.set(pos, 1);
            } else if (c == '^') {
                map.start = pos;
            }
            x += 1;
        }
        y += 1;
    }

    //std.log.info("start: {d} {d}", .{ map.start.x, map.start.y });

    return map;
}
