namespace day15;

public static class Parser
{
    public static IEnumerable<string> ParseRawInitSequence(string filename)
    {
        return File.ReadAllLines(filename)
            .Single()
            .Split(',', StringSplitOptions.TrimEntries);
    }

    public static IEnumerable<InitStep> ParseInitSequence(string filename)
    {
        return ParseRawInitSequence(filename)
            .Select(ParseInitStep);
    }

    private static InitStep ParseInitStep(string input)
    {
        if (input.EndsWith('-'))
           return new RemoveLensStep(input.TrimEnd('-'));
        
        if (input.Contains('='))
        {
            var parts = input.Split('=');
            return new AddLensStep(parts[0], Int32.Parse(parts[1]));
        }

        throw new ArgumentException("Invalid init step");
    }
}