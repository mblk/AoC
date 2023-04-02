namespace day09;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        var data = Parser.Parse(inputFile);
        
        (int shortestRouteCost, int longestRouteCost) = new BruteForceSolver(data)
            .Solve();
        
        Console.WriteLine($"Part1: {shortestRouteCost}");
        Console.WriteLine($"Part2: {longestRouteCost}");
    }
}