namespace day04;

public static class Program
{
    public static void Main(string[] args)
    {
        var cards = File.ReadAllLines(args[0])
            .Select(Card.Parse)
            .ToArray();

        DistributeCounts(cards);

        var totalPoints = cards
            .Select(c => c.CalculatePoints())
            .Sum();
        var totalCount = cards
            .Select(c => c.Count)
            .Sum();

        Console.WriteLine($"Part1: {totalPoints}");
        Console.WriteLine($"Part2: {totalCount}");
    }

    private static void DistributeCounts(IReadOnlyList<Card> cards)
    {
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
        {
            var card = cards[cardIndex];
            var correctNumbers = card.CountCorrectNumbers();

            for (int offset = 1; offset <= correctNumbers; offset++)
            {
                Card otherCard = cards[cardIndex + offset];
                otherCard.Count += card.Count;
            }
        }
    }
}