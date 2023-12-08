namespace day07;

public enum HandClassification
{
    FiveOfAKind = 7,
    FourOfAKind = 6,
    FullHouse = 5,
    ThreeOfAKind = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1,
}

public class HandClassifier
{
    private readonly bool _useJokers;

    public HandClassifier(bool useJokers)
    {
        _useJokers = useJokers;
    }

    public HandClassification Classify(Hand hand)
    {
        int jokerCount = hand.Cards
            .Count(IsJoker);

        var counts = hand.Cards
            .Where(c => !IsJoker(c))
            .GroupBy(c => c, (_, cards) => cards.Count())
            .OrderByDescending(count => count)
            .ToArray();

        int highCount = (counts.Length > 0 ? counts[0] : 0) + jokerCount;
        int lowCount = counts.Length > 1 ? counts[1] : 0;

        return highCount switch
        {
            5 => HandClassification.FiveOfAKind,
            4 => HandClassification.FourOfAKind,
            3 when lowCount == 2 => HandClassification.FullHouse,
            3 => HandClassification.ThreeOfAKind,
            2 when lowCount == 2 => HandClassification.TwoPair,
            2 => HandClassification.OnePair,
            _ => HandClassification.HighCard
        };
    }

    private bool IsJoker(Card card)
    {
        return _useJokers && card == Card.Jack;
    }
}