const std = @import("std");
const mvzr = @import("mvzr"); // mvzr: The Minimum Viable Zig Regex Library

pub fn main() !void {
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();
    const alloc = arena.allocator();

    const args = try std.process.argsAlloc(alloc);
    if (args.len != 2) {
        std.log.err("missing filename", .{});
        return;
    }

    var file = try std.fs.cwd().openFile(args[1], .{});
    defer file.close();

    const input = try file.readToEndAlloc(alloc, 1024 * 1024);

    std.log.info("part1: {d}", .{processMuls(input)});
    std.log.info("part2: {d}", .{processMulsWithEnableDisable(input)});
}

fn processMuls(input: []const u8) u32 {
    const regex_def = "mul\\(\\d{1,3},\\d{1,3}\\)";
    const regex = mvzr.compile(regex_def).?;

    var iter = regex.iterator(input);
    var total_sum: u32 = 0;

    while (iter.next()) |match| {
        total_sum += parseMul(match.slice);
    }

    return total_sum;
}

fn processMulsWithEnableDisable(input: []const u8) u32 {
    const regex_def = "mul\\(\\d{1,3},\\d{1,3}\\)|do\\(\\)|don't\\(\\)";
    const regex = mvzr.compile(regex_def).?;

    var iter = regex.iterator(input);
    var total_sum: u32 = 0;
    var muls_enabled = true;

    while (iter.next()) |match| {
        if (std.mem.eql(u8, match.slice, "do()")) {
            muls_enabled = true;
        } else if (std.mem.eql(u8, match.slice, "don't()")) {
            muls_enabled = false;
        } else {
            const value = parseMul(match.slice);
            if (muls_enabled) {
                total_sum += value;
            }
        }
    }

    return total_sum;
}

fn parseMul(input: []const u8) u32 {
    // input: mul(123,456)

    const mul_args: []const u8 = input[4 .. input.len - 1];
    // mul_args: 123,456

    const index = std.mem.indexOf(u8, mul_args, &[1]u8{','}).?;
    const left_str = mul_args[0..index];
    const right_str = mul_args[index + 1 ..];

    const left_value = std.fmt.parseInt(u32, left_str, 10) catch unreachable;
    const right_value = std.fmt.parseInt(u32, right_str, 10) catch unreachable;

    return left_value * right_value;
}

test "parseMul" {
    try std.testing.expect(parseMul("mul(2,4)") == 8);
    try std.testing.expect(parseMul("mul(5,5)") == 25);
    try std.testing.expect(parseMul("mul(3,100)") == 300);
    try std.testing.expect(parseMul("mul(999,999)") == 998001);
}
