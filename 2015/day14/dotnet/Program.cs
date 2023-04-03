namespace day14;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        ReindeerStats[] stats = Parser.Parse(inputFile);

        var simulator = new Simulator(stats);

        var result = simulator.Simulate(2503);
        
        Console.WriteLine($"Part1: {result.MaxDistanceAtEnd}");
        Console.WriteLine($"Part2: {result.MaxPoints}");
    }
}