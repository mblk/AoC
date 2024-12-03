const std = @import("std");

pub fn build(b: *std.Build) void {
    const exe = b.addExecutable(.{
        .name = "day03",
        .root_source_file = b.path("src/main.zig"),
        .target = b.host,
    });

    const mvzr = b.dependency("mvzr", .{});
    exe.root_module.addImport("mvzr", mvzr.module("mvzr"));

    b.installArtifact(exe);
}
