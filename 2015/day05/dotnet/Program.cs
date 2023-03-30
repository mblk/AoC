
// part1
{
    var niceStrings = 0;
    var vowels = "aeiou".ToCharArray();
    var evils = new string[] { "ab", "cd", "pq", "xy" };

    foreach (var line in File.ReadLines(@"../../../input.txt"))
    {
        var chars = line.ToCharArray();

        var numVowels = chars.Count(c => vowels.Contains(c));
        if (numVowels < 3) continue;

        var letterInRow = false;
        for (int i = 0; i < line.Length - 1; i++)
        {
            if (line[i] == line[i + 1])
            {
                letterInRow = true;
                break;
            }
        }
        if (!letterInRow) continue;

        var containsEvil = evils.Any(e => line.Contains(e));
        if (containsEvil) continue;

        niceStrings++;
    }

    Console.WriteLine($"Nice strings: {niceStrings}");
}

// part2
{
    var niceStrings = 0;
    
    foreach (var line in File.ReadLines(@"../../../input.txt"))
    {
        var hasDoublePair = false;
        for (int i = 0; i < line.Length - 3; i++)
        {
            var pair = line.Substring(i, 2);
            if (line.Substring(i + 2).Contains(pair))
            {
                hasDoublePair = true;
                break;
            }
        }
        if (!hasDoublePair) continue;

        var hasRepeatingWithOneInBetween = false;
        for (int i = 0; i < line.Length - 2; i++)
        {
            if (line[i] == line[i + 2])
            {
                hasRepeatingWithOneInBetween = true;
                break;
            }
        }
        if (!hasRepeatingWithOneInBetween) continue;
        
        niceStrings++;
    }
    
    Console.WriteLine($"Nice strings: {niceStrings}");
}