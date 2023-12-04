namespace day04;

public class Card
{
    public required int Id { get; init; }
    public required int[] Winning { get; init; }
    public required int[] Have { get; init; }
    public required int Count { get; set; }

    public int CountCorrectNumbers()
    {
        return Have.Count(Winning.Contains);
    }

    public int CalculatePoints()
    {
        var correctNumbers = CountCorrectNumbers();
        return correctNumbers > 0
            ? 1 << (correctNumbers - 1)
            : 0;
    }

    public static Card Parse(string input)
    {
        var sections = input.Split(new[] { ':', '|' }, 3, StringSplitOptions.TrimEntries);
        if (sections.Length != 3) throw new ArgumentException("Syntax error");

        return new Card
        {
            Id = ParseId(sections[0]),
            Winning = ParseNumbers(sections[1]),
            Have = ParseNumbers(sections[2]),
            Count = 1,
        };
    }

    private static int ParseId(string input)
    {
        if (!input.StartsWith("Card ")) throw new ArgumentException("Syntax error");
        return Int32.Parse(input[5..]);
    }

    private static int[] ParseNumbers(string input)
    {
        return input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Int32.Parse)
            .ToArray();
    }
}