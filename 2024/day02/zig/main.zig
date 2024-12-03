const std = @import("std");

pub fn main() !void {
    var gpa_state = std.heap.GeneralPurposeAllocator(.{}){};
    const gpa = gpa_state.allocator();
    defer _ = gpa_state.deinit();

    const args = try std.process.argsAlloc(gpa);
    defer std.process.argsFree(gpa, args);

    if (args.len != 2) {
        std.log.err("missing filename", .{});
        return;
    }

    const reports = try parseReports(gpa, args[1]);
    defer {
        for (reports) |r| {
            gpa.free(r.levels);
        }
        gpa.free(reports);
    }

    var num_safe_reports: u32 = 0;
    for (reports) |r| {
        if (r.isSafe()) {
            num_safe_reports += 1;
        }
    }
    std.log.info("part1: {d}", .{num_safe_reports});

    var num_safe_reports_with_dampener: u32 = 0;
    for (reports) |r| {
        if (r.isSafeWithProblemDampener()) {
            num_safe_reports_with_dampener += 1;
        }
    }
    std.log.info("part2: {d}", .{num_safe_reports_with_dampener});
}

fn parseReports(allocator: std.mem.Allocator, file_name: []const u8) ![]const Report {
    const file = try std.fs.cwd().openFile(file_name, .{});
    defer file.close();

    var buffered_reader = std.io.bufferedReader(file.reader());
    var reader = buffered_reader.reader();

    var reports = std.ArrayList(Report).init(allocator);
    defer reports.deinit();
    var levels = std.ArrayList(u32).init(allocator);
    defer levels.deinit();

    var buffer: [1000]u8 = undefined;
    while (try reader.readUntilDelimiterOrEof(&buffer, '\n')) |line| {
        var seq = std.mem.splitScalar(u8, line, ' ');

        while (seq.next()) |number| {
            const level: u32 = try std.fmt.parseInt(u32, number, 10);
            try levels.append(level);
        }

        try reports.append(Report{
            .levels = try levels.toOwnedSlice(),
        });
    }

    return reports.toOwnedSlice(); // toOwnedSlice: caller owns returned memory
}

const Report = struct {
    levels: []const u32,

    fn getDiff(a: u32, b: u32) u32 {
        if (a > b)
            return a - b;
        return b - a;
    }

    fn isSafeDiff(a: u32, b: u32) bool {
        const diff = getDiff(a, b);

        return 1 <= diff and diff <= 3;
    }

    fn isSafe(self: Report) bool {
        if (self.levels.len < 2) {
            return true;
        }

        var prev_level = self.levels[0];
        var is_ascending = true;
        var is_descending = true;

        for (self.levels[1..]) |level| {
            if (!isSafeDiff(prev_level, level)) {
                return false;
            }

            if (level > prev_level) {
                is_descending = false;
            }
            if (level < prev_level) {
                is_ascending = false;
            }

            prev_level = level;
        }

        return is_ascending or is_descending;
    }

    fn isSafeWithIgnore(self: Report, ignore_index: usize) bool {
        if (self.levels.len < 2) {
            return true;
        }

        const start_index: usize = if (ignore_index == 0) 2 else 1;
        var prev_level = if (ignore_index == 0) self.levels[1] else self.levels[0];
        var is_ascending = true;
        var is_descending = true;

        for (start_index..self.levels.len) |i| {
            if (i == ignore_index) {
                continue;
            }

            const level = self.levels[i];

            if (!isSafeDiff(prev_level, level))
                return false;

            if (level > prev_level) {
                is_descending = false;
            }
            if (level < prev_level) {
                is_ascending = false;
            }

            prev_level = level;
        }

        return is_ascending or is_descending;
    }

    fn isSafeWithProblemDampener(self: Report) bool {
        if (self.isSafe()) {
            return true;
        }

        for (0..self.levels.len) |ignore_index| {
            if (self.isSafeWithIgnore(ignore_index)) {
                return true;
            }
        }

        return false;
    }
};

test "Report isSafe" {
    const Data = struct {
        is_safe: bool,
        levels: []const u32,
    };

    // zig fmt: off
    const data: []const Data = &[_]Data{
        Data{ .is_safe = true,  .levels = &[_]u32{} },                  // no items
        Data{ .is_safe = true,  .levels = &[_]u32{1} },                 // only 1 item
        Data{ .is_safe = true,  .levels = &[_]u32{ 1, 2, 3 } },         // asc
        Data{ .is_safe = false, .levels = &[_]u32{ 1, 2, 2, 3 } },      // asc, not changing
        Data{ .is_safe = true,  .levels = &[_]u32{ 1, 2, 5, 6 } },      // asc, changing by at most 3
        Data{ .is_safe = false, .levels = &[_]u32{ 1, 2, 6, 7 } },      // asc, changing by 4
        Data{ .is_safe = false, .levels = &[_]u32{ 1, 2, 3, 2 } },      // not asc, not desc
        Data{ .is_safe = true,  .levels = &[_]u32{ 6, 5, 4 } },         // desc
        Data{ .is_safe = false, .levels = &[_]u32{ 6, 5, 5, 4 } },      // desc, not changing
        Data{ .is_safe = true,  .levels = &[_]u32{ 10, 9, 6, 5, 4 } },  // desc by at most 3
        Data{ .is_safe = false, .levels = &[_]u32{ 10, 9, 5, 4, 3 } },  // desc by 4
    };
    // zig fmt: on

    for (data) |d| {
        std.debug.print("Testing: {any}\n", .{d});

        const report = Report{
            .levels = d.levels,
        };

        try std.testing.expect(d.is_safe == report.isSafe());
    }
}

test "Report isSafeWithIgnore" {
    const Data = struct {
        is_safe: bool,
        ignore: usize,
        levels: []const u32,
    };

    // zig fmt: off
    const data: []const Data = &[_]Data{
        Data{ .is_safe = true,  .ignore = 1000, .levels = &[_]u32{ 7, 6, 4, 2, 1 } },
        Data{ .is_safe = false, .ignore = 1000, .levels = &[_]u32{ 1, 2, 7, 8, 9 } },
        Data{ .is_safe = false, .ignore = 1000, .levels = &[_]u32{ 9, 7, 6, 2, 1 } },
        Data{ .is_safe = true,  .ignore = 1,    .levels = &[_]u32{ 1, 3, 2, 4, 5 } },
        Data{ .is_safe = true,  .ignore = 2,    .levels = &[_]u32{ 8, 6, 4, 4, 1 } },
        Data{ .is_safe = true,  .ignore = 1000, .levels = &[_]u32{ 1, 3, 6, 7, 9 } },

        Data{ .is_safe = true,  .ignore = 0, .levels = &[_]u32{ 27, 24, 26, 29, 30, 33, 36 } },
    };
    // zig fmt: on

    for (data) |d| {
        std.debug.print("Testing: {any}\n", .{d});

        const report = Report{
            .levels = d.levels,
        };

        try std.testing.expect(d.is_safe == report.isSafeWithIgnore(d.ignore));
    }
}

test "Report isSafeWithProblemDampener" {
    const Data = struct {
        is_safe: bool,
        levels: []const u32,
    };

    // zig fmt: off
    const data: []const Data = &[_]Data{
        Data{ .is_safe = true,  .levels = &[_]u32{ 7, 6, 4, 2, 1 } },
        Data{ .is_safe = false, .levels = &[_]u32{ 1, 2, 7, 8, 9 } },
        Data{ .is_safe = false, .levels = &[_]u32{ 9, 7, 6, 2, 1 } },
        Data{ .is_safe = true,  .levels = &[_]u32{ 1, 3, 2, 4, 5 } },
        Data{ .is_safe = true,  .levels = &[_]u32{ 8, 6, 4, 4, 1 } },
        Data{ .is_safe = true,  .levels = &[_]u32{ 1, 3, 6, 7, 9 } },

    };
    // zig fmt: on

    for (data) |d| {
        std.debug.print("Testing: {any}\n", .{d});

        const report = Report{
            .levels = d.levels,
        };

        try std.testing.expect(d.is_safe == report.isSafeWithProblemDampener());
    }
}
