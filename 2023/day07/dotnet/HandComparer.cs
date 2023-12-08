namespace day07;

public class HandComparer : IComparer<Hand>
{
    private readonly HandClassifier _handClassifier;
    private readonly CardComparer _cardComparer;

    public HandComparer(bool useJokers)
    {
        _handClassifier = new HandClassifier(useJokers);
        _cardComparer = new CardComparer(useJokers);
    }

    public int Compare(Hand? hand1, Hand? hand2)
    {
        if (ReferenceEquals(hand1, hand2)) return 0;
        if (ReferenceEquals(null, hand2)) return 1;
        if (ReferenceEquals(null, hand1)) return -1;

        var c1 = _handClassifier.Classify(hand1);
        var c2 = _handClassifier.Classify(hand2);
        if (c1 != c2)
            return c1.CompareTo(c2);

        var numCards = Math.Min(hand1.Cards.Length, hand2.Cards.Length);
        for (var i = 0; i < numCards; i++)
        {
            var card1 = hand1.Cards[i];
            var card2 = hand2.Cards[i];
            if (card1 != card2)
                return _cardComparer.Compare(card1, card2);
        }

        return 0;
    }
}