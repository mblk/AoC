namespace day16;

public static class Program
{
    public static void Main(string[] args)
    {
        var map = MapParser.ParseMap(args[0]);

        var start = new LightFront(new Position(0, 0), Direction.Right);
        var part1 = LightFrontSimulator.Simulate(map, start);
        Console.WriteLine($"Part1: {part1}");

        var part2 = LightFrontSimulator.FindMaxEnergized(map);
        Console.WriteLine($"Part2: {part2}");
    }
}