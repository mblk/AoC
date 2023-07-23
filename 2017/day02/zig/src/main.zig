const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    var allocator = gpa.allocator();

    var arg_iter = std.process.args();
    _ = arg_iter.next().?;
    var filename: []const u8 = arg_iter.next().?;

    std.log.info("Part1: {any}", .{try calculate_checksum(filename, allocator, part1_checksum_logic)});
    std.log.info("Part2: {any}", .{try calculate_checksum(filename, allocator, part2_checksum_logic)});
}

fn part1_checksum_logic(values: []u32) u32 {
    var min_value: u32 = values[0];
    var max_value: u32 = values[0];
    for (values) |x| {
        if (x < min_value) min_value = x;
        if (x > max_value) max_value = x;
    }
    return max_value - min_value;
}

fn part2_checksum_logic(values: []u32) u32 {
    for (0..values.len) |idx1| {
        for (0..values.len) |idx2| {
            if (idx1 == idx2) continue;

            const val1 = values[idx1];
            const val2 = values[idx2];

            if (val1 % val2 == 0) return val1 / val2;
            if (val2 % val1 == 0) return val2 / val1;
        }
    }
    unreachable;
}

fn calculate_checksum(filename: []const u8, allocator: std.mem.Allocator, checksum_fn: *const fn ([]u32) u32) !u32 {
    const file = try std.fs.cwd().openFile(filename, .{});
    defer file.close();
    var buf_reader = std.io.bufferedReader(file.reader());
    var reader = buf_reader.reader(); // Note: could also use file.reader() directly without buffer.
    var checksum: u32 = 0;

    while (true) {
        var buf: [1024]u8 = undefined;
        var fbs = std.io.fixedBufferStream(&buf);

        reader.streamUntilDelimiter(fbs.writer(), '\n', fbs.buffer.len) catch |err| {
            if (err == error.EndOfStream) {
                break;
            } else {
                return err;
            }
        };

        const line: []u8 = fbs.getWritten();
        var iter = std.mem.splitAny(u8, line, " \t");
        var values = std.ArrayList(u32).init(allocator);
        defer values.deinit();

        while (iter.next()) |item| {
            const item_value: u32 = try std.fmt.parseInt(u32, item, 10);
            try values.append(item_value);
        }

        checksum += checksum_fn(values.items);
    }

    return checksum;
}
