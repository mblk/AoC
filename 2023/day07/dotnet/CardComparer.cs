namespace day07;

public class CardComparer : IComparer<Card>
{
    private readonly bool _useJokers;

    public CardComparer(bool useJokers)
    {
        _useJokers = useJokers;
    }

    public int Compare(Card card1, Card card2)
    {
        var value1 = GetValue(card1);
        var value2 = GetValue(card2);
        return value1.CompareTo(value2);
    }

    private int GetValue(Card card)
    {
        if (_useJokers && card == Card.Jack)
            return 1;
        return (int)card;
    }
}