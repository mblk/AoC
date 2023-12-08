namespace day07.tests;

public class HandClassifierTests
{
    [TestCase(HandClassification.FiveOfAKind, "22222")]
    [TestCase(HandClassification.FourOfAKind, "22223")]
    [TestCase(HandClassification.FullHouse, "22233")]
    [TestCase(HandClassification.ThreeOfAKind, "22234")]
    [TestCase(HandClassification.TwoPair, "22334")]
    [TestCase(HandClassification.OnePair, "22345")]
    [TestCase(HandClassification.HighCard, "23456")]
    public void Should_ClassifyHand(HandClassification expectedClassification, string handData)
    {
        var hand = HandParser.ParseHand(handData + " 0");
        var classification = new HandClassifier(false).Classify(hand);
        Assert.That(classification, Is.EqualTo(expectedClassification));
    }

    [TestCase(HandClassification.FiveOfAKind, "22222")]
    [TestCase(HandClassification.FiveOfAKind, "2222J")]
    [TestCase(HandClassification.FiveOfAKind, "222JJ")]
    [TestCase(HandClassification.FiveOfAKind, "22JJJ")]
    [TestCase(HandClassification.FiveOfAKind, "2JJJJ")]
    [TestCase(HandClassification.FiveOfAKind, "JJJJJ")]
    [TestCase(HandClassification.FourOfAKind, "22223")]
    [TestCase(HandClassification.FourOfAKind, "222J3")]
    [TestCase(HandClassification.FourOfAKind, "22JJ3")]
    [TestCase(HandClassification.FourOfAKind, "2JJJ3")]
    [TestCase(HandClassification.FullHouse, "22233")]
    [TestCase(HandClassification.FullHouse, "22J33")]
    [TestCase(HandClassification.ThreeOfAKind, "22234")]
    [TestCase(HandClassification.ThreeOfAKind, "22J34")]
    [TestCase(HandClassification.ThreeOfAKind, "2JJ34")]
    [TestCase(HandClassification.TwoPair, "22334")]
    [TestCase(HandClassification.OnePair, "22345")]
    [TestCase(HandClassification.OnePair, "2J345")]
    [TestCase(HandClassification.HighCard, "23456")]
    public void Should_ClassifyHand_WithJokers(HandClassification expectedClassification, string handData)
    {
        var hand = HandParser.ParseHand(handData + " 0");
        var classification = new HandClassifier(true).Classify(hand);
        Assert.That(classification, Is.EqualTo(expectedClassification));
    }
}