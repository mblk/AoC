namespace day16;

internal class FactChecker
{
    private readonly Aunt[] _aunts;

    public FactChecker(Aunt[] aunts)
    {
        _aunts = aunts;
    }
    
    public Aunt[] CheckAunts(Fact[] targetFacts, bool useImprovedCheck = false)
    {
        var targetFactsDict = targetFacts
            .ToDictionary(f => f.Id, f => f.Value);

        var matchingAunts = new List<Aunt>();
        
        foreach (var aunt in _aunts)
        {
            var auntIsMatch = true;

            foreach (var fact in aunt.Facts)
            {
                if (targetFactsDict.TryGetValue(fact.Id, out int targetValue) &&
                    !CheckFact(fact, targetValue, useImprovedCheck))
                    auntIsMatch = false;
            }

            if (auntIsMatch)
                matchingAunts.Add(aunt);
        }

        return matchingAunts.ToArray();
    }

    private bool CheckFact(Fact factToCheck, int targetValue, bool useImprovedCheck)
    {
        if (useImprovedCheck)
        {
            switch (factToCheck.Id)
            {
                case "cats":
                case "trees":
                    return factToCheck.Value > targetValue;

                case "pomeranians":
                case "goldfish":
                    return factToCheck.Value < targetValue;
                
                default:
                    return factToCheck.Value == targetValue;
            }
        }
        else
        {
            return factToCheck.Value == targetValue;
        }
    }
}