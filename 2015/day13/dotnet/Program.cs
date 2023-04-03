namespace day13;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        var seatingInfos = Parser.Parse(inputFile);
        var seatingSolver = new SeatingSolver(seatingInfos);
        
        string[] names = seatingInfos
            .Select(x => x.Person)
            .Distinct()
            .OrderBy(s => s)
            .ToArray();

        Console.WriteLine($"Part1: {seatingSolver.FindSolution(names)}");

        names = names.Concat(new[] { "me", }).ToArray();

        Console.WriteLine($"Part2: {seatingSolver.FindSolution(names)}");
    }
}
