namespace day16;

internal record Aunt(string Name, Fact[] Facts);
internal record Fact(string Id, int Value);

internal static class Parser
{
    internal static Aunt[] Parse(string filename)
    {
        var aunts = new List<Aunt>();

        foreach (string line in File.ReadLines(filename))
        {
            // Sue 468: goldfish: 2, children: 4, trees: 1

            var colonIndex = line.IndexOf(':');

            var name = line[..colonIndex];

            var facts = line[(colonIndex + 1)..]
                .Trim()
                .Split(',')
                .Select(ParseFact)
                .ToArray();
            
            aunts.Add(new Aunt(name, facts));
        }
            
        return aunts.ToArray();
    }

    private static Fact ParseFact(string input)
    {
        string[] parts = input.Split(':')
            .Select(s => s.Trim())
            .ToArray();

        if (parts.Length != 2) throw new Exception("invalid input");
        
        if (!Int32.TryParse(parts[1], out int value)) throw new Exception("invalid input");

        return new Fact(parts[0], value);
    }
}