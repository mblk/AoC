const std = @import("std");

const Rule = struct {
    a: u32,
    b: u32,
};

const Update = struct {
    id: u32,
    pages: []u32,

    fn getMiddlePage(self: Update) u32 {
        const middle_index = self.pages.len / 2;
        return self.pages[middle_index];
    }
};

const Data = struct {
    rules: []Rule,
    updates: []Update,
};

pub fn main() !void {
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();
    const allocator = arena.allocator();

    const args = try std.process.argsAlloc(allocator);
    if (args.len != 2) {
        std.log.err("missing filename", .{});
        return;
    }

    const data = try parseInput(args[1], allocator);

    var goodUpdatesMiddlePageSum: u32 = 0;
    var badUpdatesMiddlePageSum: u32 = 0;

    for (data.updates) |update| {
        if (checkUpdate(update, data.rules)) {
            goodUpdatesMiddlePageSum += update.getMiddlePage();
        } else {
            fixUpdate(update, data.rules);
            badUpdatesMiddlePageSum += update.getMiddlePage();
        }
    }

    std.log.info("part1: {d}", .{goodUpdatesMiddlePageSum});
    std.log.info("part2: {d}", .{badUpdatesMiddlePageSum});
}

fn checkUpdate(update: Update, rules: []Rule) bool {
    for (rules) |rule| {
        var a_index: ?usize = null;
        var b_index: ?usize = null;

        for (update.pages, 0..) |page, i| {
            if (page == rule.a) {
                std.debug.assert(a_index == null);
                a_index = i;
            }
            if (page == rule.b) {
                std.debug.assert(b_index == null);
                b_index = i;
            }
        }

        if (a_index != null and b_index != null and a_index.? > b_index.?) {
            return false;
        }

        // if (a_index) |a| {
        //     if (b_index) |b| {
        //         if (a > b) {
        //             return false;
        //         }
        //     }
        // }
    }

    return true;
}

fn fixUpdate(update: Update, rules: []Rule) void {
    var is_valid = false;
    while (!is_valid) {
        is_valid = true;

        for (rules) |rule| {
            var a_index: ?usize = null;
            var b_index: ?usize = null;

            for (update.pages, 0..) |page, i| {
                if (page == rule.a) {
                    std.debug.assert(a_index == null);
                    a_index = i;
                }
                if (page == rule.b) {
                    std.debug.assert(b_index == null);
                    b_index = i;
                }
            }

            if (a_index) |a| {
                if (b_index) |b| {
                    if (a > b) {
                        const temp = update.pages[a];
                        update.pages[a] = update.pages[b];
                        update.pages[b] = temp;
                        is_valid = false;
                    }
                }
            }
        }
    }
}

fn parseInput(filename: []const u8, allocator: std.mem.Allocator) !Data {
    var file = try std.fs.cwd().openFile(filename, .{});
    defer file.close();

    var buffered_reader = std.io.bufferedReader(file.reader());
    var reader = buffered_reader.reader();

    var reading_rules = true;
    var rules = std.ArrayList(Rule).init(allocator);
    defer rules.deinit();
    var updates = std.ArrayList(Update).init(allocator);
    defer updates.deinit();
    var temp_page_numbers = std.ArrayList(u32).init(allocator);
    defer temp_page_numbers.deinit();

    var buffer: [128]u8 = undefined;
    while (try reader.readUntilDelimiterOrEof(&buffer, '\n')) |line| {
        if (reading_rules and line.len == 0) {
            reading_rules = false;
        } else if (reading_rules) {
            const sep: usize = std.mem.indexOfScalar(u8, line, '|').?;
            const a: u32 = try std.fmt.parseInt(u32, line[0..sep], 10);
            const b: u32 = try std.fmt.parseInt(u32, line[sep + 1 ..], 10);

            try rules.append(Rule{
                .a = a,
                .b = b,
            });
        } else {
            var iter = std.mem.splitScalar(u8, line, ',');

            temp_page_numbers.clearRetainingCapacity();
            while (iter.next()) |item| {
                const number = try std.fmt.parseInt(u32, item, 10);
                try temp_page_numbers.append(number);
            }

            try updates.append(Update{
                .id = @intCast(updates.items.len),
                .pages = try temp_page_numbers.toOwnedSlice(),
            });
        }
    }

    return Data{
        .rules = try rules.toOwnedSlice(),
        .updates = try updates.toOwnedSlice(),
    };
}
