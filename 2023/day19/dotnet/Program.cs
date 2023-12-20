using day19.Data;

namespace day19;

public static class Program
{
    public static void Main(string[] args)
    {
        var (workflows, parts) = Parser.ParseWorkflowsAndParts(args[0]);

        var acceptedParts = new PartProcessor(workflows)
            .ProcessParts(parts)
            .ToArray();

        var part1 = acceptedParts.Sum(p => p.GetSumOfValues());
        Console.WriteLine($"Part1: {part1}");
        
        var startingPartRange = new PartRange(
            new ValueRange(1, 4000), new ValueRange(1, 4000),
            new ValueRange(1, 4000), new ValueRange(1, 4000));

        var acceptedPartRanges = new PartRangeProcessor(workflows)
            .ProcessPartRange(startingPartRange)
            .ToArray();

        var part2 = acceptedPartRanges.Sum(r => r.GetProductOfRanges());
        Console.WriteLine($"Part2: {part2}");
    }
}