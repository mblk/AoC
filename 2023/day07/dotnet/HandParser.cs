namespace day07;

public static class HandParser
{
    public static Hand ParseHand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.TrimEntries);
        if (parts.Length != 2) throw new ArgumentException("Syntax error");

        var cards = parts[0].ToCharArray().Select(ParseCard).ToArray();
        var bid = Int32.Parse(parts[1]);

        return new Hand(cards, bid);
    }

    private static Card ParseCard(char input)
    {
        return input switch
        {
            'A' => Card.Ace,
            'K' => Card.King,
            'Q' => Card.Queen,
            'J' => Card.Jack,
            'T' => Card.Ten,
            '9' => Card.Nine,
            '8' => Card.Eight,
            '7' => Card.Seven,
            '6' => Card.Six,
            '5' => Card.Five,
            '4' => Card.Four,
            '3' => Card.Three,
            '2' => Card.Two,
            _ => throw new ArgumentException($"Invalid card: {input}"),
        };
    }
}