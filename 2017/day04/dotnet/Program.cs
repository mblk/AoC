namespace day04;

public static class Program
{
    public static void Main(string[] args)
    {
        var lines = File.ReadAllLines("../../../../input");

        Console.WriteLine($"Total: {lines.Length}");
        Console.WriteLine($"Part1: {lines.Count(IsValidPassphrase)}");
        Console.WriteLine($"Part2: {lines.Count(IsValidPassphraseNew)}");
    }

    private static bool IsValidPassphrase(string passphrase)
    {
        return passphrase
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(s => s)
            .All(g => g.Count() == 1);
    }

    private static bool IsValidPassphraseNew(string passphrase)
    {
        var words = passphrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            for (int j = 0; j < words.Length; j++)
            {
                if (i == j) continue;
                
                var word1 = words[i];
                var word2 = words[j];
                
                if (IsAnagram(word1, word2))
                    return false;
            }
        }

        return true;
    }

    private static bool IsAnagram(string a, string b)
    {
        if (a.Length != b.Length) return false;

        var g1 = a.ToCharArray()
            .GroupBy(c => c)
            .OrderBy(g => g.Key)
            .ToArray();

        var g2 = b.ToCharArray()
            .GroupBy(c => c)
            .OrderBy(g => g.Key)
            .ToArray();

        if (g1.Length != g2.Length) return false;
        
        for (int i = 0; i < g1.Length; i++)
        {
            if (g1[i].Key != g2[i].Key) return false;
            if (g1[i].Count() != g2[i].Count()) return false;
        }
        
        return true;
    }
}
