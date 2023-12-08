namespace day07;

public static class Program
{
    public static void Main(string[] args)
    {
        var hands = File.ReadAllLines(args[0])
            .Select(HandParser.ParseHand)
            .ToArray();

        Console.WriteLine($"Part1: {GetTotalWinnings(hands, false)}");
        Console.WriteLine($"Part2: {GetTotalWinnings(hands, true)}");
    }

    private static int GetTotalWinnings(IReadOnlyList<Hand> hands, bool useJokers)
    {
        var sortedHands = hands
            .OrderBy(h => h, new HandComparer(useJokers))
            .ToArray();

        var rankedHands = Enumerable.Range(1, hands.Count)
            .Zip(sortedHands, (r, h) => new { Rank = r, Hand = h })
            .ToArray();

        var totalWinnings = rankedHands
            .Select(p => p.Rank * p.Hand.Bid)
            .Sum();

        return totalWinnings;
    }
}