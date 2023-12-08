namespace day07;

public record Hand(Card[] Cards, int Bid)
{
    public override string ToString()
    {
        var cards = String.Join(',', Cards.Select(c => c.ToString()));
        return $"Hand {cards} for {Bid}";
    }
}