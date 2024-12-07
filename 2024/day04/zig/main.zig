const std = @import("std");

const Position = struct {
    x: i32,
    y: i32,

    fn createFrom(comptime T: type, x: T, y: T) Position {
        return Position{
            .x = @intCast(x),
            .y = @intCast(y),
        };
    }

    fn move(self: Position, dir: Direction, count: i32) Position {
        return Position{
            .x = self.x + dir.dx * count,
            .y = self.y + dir.dy * count,
        };
    }
};

const Direction = struct {
    dx: i32,
    dy: i32,

    fn create(dx: i32, dy: i32) Direction {
        return Direction{
            .dx = dx,
            .dy = dy,
        };
    }

    fn equals(self: Direction, other: Direction) bool {
        return self.dx == other.dx and self.dy == other.dy;
    }

    fn getAll() [8]Direction {
        return [8]Direction{
            create(1, 0),
            create(-1, 0),
            create(0, 1),
            create(0, -1),

            create(1, 1),
            create(-1, 1),
            create(1, -1),
            create(-1, -1),
        };
    }

    fn getAllDiag() [4]Direction {
        return [4]Direction{
            create(1, 1),
            create(-1, 1),
            create(1, -1),
            create(-1, -1),
        };
    }
};

const Map = struct {
    width: usize,
    height: usize,
    data: []u8,

    fn create(allocator: std.mem.Allocator, width: usize, height: usize) !Map {
        return Map{
            .width = width,
            .height = height,
            .data = try allocator.alloc(u8, width * height),
        };
    }

    fn set(self: *Map, x: usize, y: usize, data: u8) void {
        const index = y * self.width + x;
        self.data[index] = data;
    }

    fn get(self: *const Map, pos: Position) u8 {
        if (pos.x < 0 or pos.y < 0 or pos.x >= self.width or pos.y >= self.height) {
            return '.';
        }

        const x: usize = @intCast(pos.x);
        const y: usize = @intCast(pos.y);
        const index: usize = y * self.width + x;

        return self.data[index];
    }

    fn getSlice(
        self: *const Map,
        buf: []u8,
        pos: Position,
        dir: Direction,
    ) void {
        for (0..buf.len) |i| {
            const p = pos.move(dir, @as(i32, @intCast(i)));
            buf[i] = self.get(p);
        }
    }

    fn getCross(
        self: *const Map,
        buf: *[3]u8,
        pos: Position,
        dir: Direction,
    ) void {
        buf[0] = self.get(pos.move(dir, -1));
        buf[1] = self.get(pos);
        buf[2] = self.get(pos.move(dir, 1));
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

    const map = try parseMap(args[1], alloc);

    std.log.info("part1: {d}", .{solvePart1(&map)});
    std.log.info("part2: {d}", .{solvePart2(&map)});
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

    std.log.info("map: w={d} h={d}", .{ map_width, map_height });

    lines.reset();

    var map = try Map.create(allocator, map_width, map_height);

    var y: usize = 0;
    while (lines.next()) |line| {
        var x: usize = 0;
        for (line) |c| {
            map.set(x, y, c);
            x += 1;
        }
        y += 1;
    }

    return map;
}

fn solvePart1(map: *const Map) usize {
    const directions = Direction.getAll();

    var xmas_count: usize = 0;
    var buf: [4]u8 = undefined;

    for (0..map.height) |y0| {
        for (0..map.width) |x0| {
            const pos = Position.createFrom(usize, x0, y0);

            for (directions) |dir| {
                map.getSlice(&buf, pos, dir);

                if (std.mem.eql(u8, &buf, "XMAS")) {
                    xmas_count += 1;
                }
            }
        }
    }

    return xmas_count;
}

fn solvePart2(map: *const Map) usize {
    const directions = Direction.getAllDiag();

    var xmas_count: usize = 0;
    var buf1: [3]u8 = undefined;
    var buf2: [3]u8 = undefined;

    for (0..map.height) |y0| {
        outer: for (0..map.width) |x0| {
            const pos = Position.createFrom(usize, x0, y0);
            if (map.get(pos) != 'A') continue;

            for (directions) |dir1| {
                map.getCross(&buf1, pos, dir1);
                if (!std.mem.eql(u8, &buf1, "MAS")) continue;

                for (directions) |dir2| {
                    if (dir1.equals(dir2)) continue;

                    map.getCross(&buf2, pos, dir2);
                    if (!std.mem.eql(u8, &buf2, "MAS")) continue;

                    xmas_count += 1;
                    continue :outer;
                }
            }
        }
    }

    return xmas_count;
}
