namespace day07.tests;

public class HandParserTests
{
    [Test]
    public void Should_ParseHand_WithAllCards()
    {
        var hand = HandParser.ParseHand("23456789TJQKA 123");

        Assert.Multiple(() =>
        {
            Assert.That(hand.Cards, Has.Exactly(13).Items);

            CollectionAssert.AreEqual(new Card[]
            {
                Card.Two, Card.Three, Card.Four, Card.Five, Card.Six,
                Card.Seven, Card.Eight, Card.Nine, Card.Ten, Card.Jack,
                Card.Queen, Card.King, Card.Ace,
            }, hand.Cards);

            Assert.That(hand.Bid, Is.EqualTo(123));
        });
    }

    [Test]
    public void Should_ParseHand_WithDuplicates()
    {
        var hand = HandParser.ParseHand("44KQQ 5");
        
        Assert.Multiple(() =>
        {
            CollectionAssert.AreEqual(new Card[]
            {
                Card.Four, Card.Four, Card.King, Card.Queen, Card.Queen
            }, hand.Cards);
            
            Assert.That(hand.Bid, Is.EqualTo(5));
        });
    }
}