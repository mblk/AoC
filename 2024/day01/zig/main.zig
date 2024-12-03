const std = @import("std");

pub fn main() !void {
    var gpa_state = std.heap.GeneralPurposeAllocator(.{}){};
    const gpa = gpa_state.allocator();
    defer _ = gpa_state.deinit();

    const args = try std.process.argsAlloc(gpa);
    defer std.process.argsFree(gpa, args);

    if (args.len != 2) {
        std.log.err("missing arg", .{});
        return;
    }

    var file = try std.fs.cwd().openFile(args[1], .{});
    defer file.close();

    const file_reader = file.reader();
    var buffered_reader_thing = std.io.bufferedReader(file_reader);
    var buffered_reader = buffered_reader_thing.reader();

    var numbers1 = std.ArrayList(u32).init(gpa);
    var numbers2 = std.ArrayList(u32).init(gpa);
    defer numbers1.deinit();
    defer numbers2.deinit();

    var buffer: [1024]u8 = undefined;
    // buffer is array
    // &buffer -> coerce to slice []u8

    while (try buffered_reader.readUntilDelimiterOrEof(&buffer, '\n')) |line| {
        var seq = std.mem.splitSequence(u8, line, "   ");

        const a: []const u8 = seq.next().?;
        const b: []const u8 = seq.next().?;

        const num1 = try std.fmt.parseInt(u32, a, 10);
        const num2 = try std.fmt.parseInt(u32, b, 10);

        try numbers1.append(num1);
        try numbers2.append(num2);
    }

    std.mem.sort(u32, numbers1.items, {}, comptime std.sort.asc(u32));
    std.mem.sort(u32, numbers2.items, {}, comptime std.sort.asc(u32));

    var total_diff: u32 = 0;
    for (numbers1.items, numbers2.items) |a, b| {
        const diff: u32 = if (a > b) a - b else b - a;
        total_diff += diff;
    }

    var total_similarity_score: u32 = 0;
    for (numbers1.items) |a| {
        var count: u32 = 0;

        for (numbers2.items) |b| {
            if (a == b) count += 1;
        }

        total_similarity_score += a * count;
    }

    std.log.info("part1: {d}", .{total_diff});
    std.log.info("part2: {d}", .{total_similarity_score});
}
