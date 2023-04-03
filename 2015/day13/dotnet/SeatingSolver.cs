namespace day13;

internal class SeatingSolver
{
    private readonly IReadOnlyDictionary<string, int> _happinessLookup;

    public SeatingSolver(SeatingInfo[] seatingInfos)
    {
        _happinessLookup = CreateHappinessLookup(seatingInfos);
    }

    public int FindSolution(string[] names)
    {
        int[] indices = new int[names.Length];

        int maxHappiness = Int32.MinValue;

        while (true)
        {
            if (IndicesAreValid(indices))
            {
                int h = CalculateHappiness(names, indices);
                if (h > maxHappiness)
                    maxHappiness = h;
            }

            if (!IncreaseIndices(indices)) break;
        }

        return maxHappiness;
    }
    
    private static IReadOnlyDictionary<string, int> CreateHappinessLookup(SeatingInfo[] seatingInfos)
    {
        var lookup = new Dictionary<string, int>();
        
        foreach (var info in seatingInfos)
        {
            AddToHappinessLookup(lookup, info.Person, info.Neighbour, info.Happiness);
            AddToHappinessLookup(lookup, info.Neighbour, info.Person, info.Happiness);
        }

        return lookup;
    }
    
    private static void AddToHappinessLookup(Dictionary<string, int> lookup, string name1, string name2, int happinessToAdd)
    {
        var key = MakeLookupKey(name1, name2);

        if (!lookup.TryGetValue(key, out int relativeHappiness))
            relativeHappiness = 0;

        relativeHappiness += happinessToAdd;
        lookup[key] = relativeHappiness;
    }

    private static string MakeLookupKey(string name, string otherName)
    {
        return $"{name}-{otherName}";
    }

    private int CalculateHappiness(string[] names, int[] indices)
    {
        int totalHappiness = 0;

        for (int i = 0; i < indices.Length; i++)
        {
            int thisIndex = indices[i];
            int nextIndex = indices[(i + 1) % indices.Length];

            string thisPerson = names[thisIndex];
            string nextPerson = names[nextIndex];

            string key = MakeLookupKey(thisPerson, nextPerson);

            if (_happinessLookup.TryGetValue(key, out int happiness))
                totalHappiness += happiness;
        }

        return totalHappiness;
    }

    private static bool IncreaseIndices(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i]++;
            if (indices[i] >= indices.Length)
            {
                indices[i] = 0;

                if (i == indices.Length - 1)
                    return false; // All combinations done.
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private static bool IndicesAreValid(int[] indices)
    {
        int[] hits = new int[indices.Length];

        for (int i = 0; i < indices.Length; i++)
            hits[indices[i]]++;

        for (int i = 0; i < indices.Length; i++)
            if (hits[i] != 1)
                return false;

        return true;
    }
}
