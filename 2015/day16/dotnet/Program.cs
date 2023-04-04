namespace day16;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        Aunt[] aunts = Parser.Parse(inputFile);

        var mfcsamFacts = new[]
        {
            new Fact("children", 3),
            new Fact("cats", 7),
            new Fact("samoyeds", 2),
            new Fact("pomeranians", 3),
            new Fact("akitas", 0),
            new Fact("vizslas", 0),
            new Fact("goldfish", 5),
            new Fact("trees", 3),
            new Fact("cars", 2),
            new Fact("perfumes", 1),
        };

        var factChecker = new FactChecker(aunts);

        var result1 = factChecker.CheckAunts(mfcsamFacts);
        var result2 = factChecker.CheckAunts(mfcsamFacts, true);
        
        Console.WriteLine($"Part1: {String.Join(",", result1.Select(a => a.Name))}");
        Console.WriteLine($"Part2: {String.Join(",", result2.Select(a => a.Name))}");
    }
}
