const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    var allocator = gpa.allocator();

    const input = try read_input_file(allocator);
    defer allocator.free(input);

    const trim_chars = [_]u8{ 0x0, 0xA, 0xD };
    const trimmed_input = std.mem.trimRight(u8, input, &trim_chars);

    std.log.info("Part1: {any}", .{calculate_captcha_sum_1(trimmed_input)});
    std.log.info("Part2: {any}", .{calculate_captcha_sum_2(trimmed_input)});
}

fn read_input_file(allocator: std.mem.Allocator) ![]const u8 {
    const file = try std.fs.cwd().openFile("../input", .{});
    defer file.close();

    const stat = try file.stat();
    const data = try file.readToEndAlloc(allocator, stat.size);

    return data;
}

fn calculate_captcha_sum_1(input: []const u8) usize {
    return calculate_captcha_sum(input, 1);
}

fn calculate_captcha_sum_2(input: []const u8) usize {
    return calculate_captcha_sum(input, input.len / 2);
}

fn calculate_captcha_sum(input: []const u8, next_offset: usize) usize {
    var i: usize = 0;
    var sum: usize = 0;

    while (i < input.len) : (i += 1) {
        const curr = get_char_value(input[i]);
        const next = get_char_value(input[(i + next_offset) % input.len]);

        if (curr == next)
            sum += curr;
    }

    return sum;
}

fn get_char_value(ch: u8) u8 {
    if (ch >= '0' and ch <= '9') {
        return ch - '0';
    } else {
        return 0;
    }
}

test "examples for part1" {
    try std.testing.expectEqual(@as(usize, 3), calculate_captcha_sum_1("1122"));
    try std.testing.expectEqual(@as(usize, 4), calculate_captcha_sum_1("1111"));
    try std.testing.expectEqual(@as(usize, 0), calculate_captcha_sum_1("1234"));
    try std.testing.expectEqual(@as(usize, 9), calculate_captcha_sum_1("91212129"));
}

test "examples for part2" {
    try std.testing.expectEqual(@as(usize, 6), calculate_captcha_sum_2("1212"));
    try std.testing.expectEqual(@as(usize, 0), calculate_captcha_sum_2("1221"));
    try std.testing.expectEqual(@as(usize, 4), calculate_captcha_sum_2("123425"));
    try std.testing.expectEqual(@as(usize, 12), calculate_captcha_sum_2("123123"));
    try std.testing.expectEqual(@as(usize, 4), calculate_captcha_sum_2("12131415"));
}
