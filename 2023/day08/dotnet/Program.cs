using System.Diagnostics;
using aoc.common;

namespace day08;

public enum Direction
{
    Left,
    Right
}

public record Config(Direction[] Directions, IReadOnlyDictionary<string, (string, string)> Nodes);

public static class Program
{
    public static void Main(string[] args)
    {
        var config = ParseConfig(args[0]);

        var part1 = Walk(config, "AAA", p => p == "ZZZ");
        Console.WriteLine($"Part1: {part1}");

        var cycleLengths = config.Nodes.Keys
            .Where(pos => pos.EndsWith("A"))
            .Select(start => Walk(config, start, p => p.EndsWith("Z")))
            .ToArray();

        var part2 = MathUtils.LeastCommonMultiple(cycleLengths);
        Console.WriteLine($"Part2: {part2}");
    }

    private static long Walk(Config config, string start, Func<string, bool> endCondition)
    {
        var totalSteps = 0;
        var currentPosition = start;

        foreach (var direction in GetEndlessDirections(config))
        {
            if (endCondition(currentPosition))
                return totalSteps;
            
            currentPosition = WalkStep(config, currentPosition, direction);
            totalSteps++;
        }

        throw new UnreachableException();
    }

    private static IEnumerable<Direction> GetEndlessDirections(Config config)
    {
        var index = 0;
        while (true)
        {
            var direction = config.Directions[index];
            index = (index + 1) % config.Directions.Length;
            yield return direction;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private static string WalkStep(Config config, string currentPosition, Direction direction)
    {
        var node = config.Nodes[currentPosition];
        return direction switch
        {
            Direction.Left => node.Item1,
            Direction.Right => node.Item2,
            _ => throw new UnreachableException(),
        };
    }

    private static Config ParseConfig(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var directions = lines[0]
            .ToCharArray()
            .Select(ParseDirection)
            .ToArray();

        var nodes = lines[2..]
            .Select(ParseNode)
            .ToDictionary(t => t.Position, t => (t.Left, t.Right));

        return new Config(directions, nodes);
    }

    private static Direction ParseDirection(char input) => input switch
    {
        'L' => Direction.Left,
        'R' => Direction.Right,
        _ => throw new ArgumentException("Invalid direction"),
    };

    private static (string Position, string Left, string Right) ParseNode(string input)
    {
        // AAA = (BBB, CCC)
        var parts = input.Split("=(,)".ToCharArray(),
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) throw new ArgumentException("Invalid node");
        return (parts[0], parts[1], parts[2]);
    }
}