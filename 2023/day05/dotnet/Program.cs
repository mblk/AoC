using aoc.common;

namespace day05;

public static class Program
{
    public static void Main(string[] args)
    {
        var config = new ConfigParser(args[0])
            .Parse();
        config.DumpToConsole();

        var valueMapper = new ValueMapper(config.Mappings);

        var seedLocations = config.Seeds
            .Select(valueMapper.MapValue)
            .ToArray();

        var seedRangeLocations = config.SeedRanges
            .SelectMany(valueMapper.MapRange)
            .ToArray();

        Console.WriteLine($"Part1: {seedLocations.Min()}");
        Console.WriteLine($"Part2: {seedRangeLocations.Min(r => r.Start)}");
    }
}