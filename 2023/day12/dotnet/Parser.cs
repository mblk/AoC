namespace day12;

public static class Parser
{
    public static SpringRow[] ParseSpringRows(string filename)
    {
        return File.ReadAllLines(filename)
            .Select(ParseSpringRow)
            .ToArray();
    }

    private static SpringRow ParseSpringRow(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.TrimEntries);

        var states = parts[0]
            .ToCharArray()
            .Select(ParseSpringState)
            .ToArray();

        var groups = parts[1]
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(Int32.Parse)
            .ToArray();

        return new SpringRow(states, groups);
    }

    private static SpringState ParseSpringState(char input) => input switch
    {
        '#' => SpringState.Damaged,
        '.' => SpringState.Operational,
        '?' => SpringState.Unknown,
        _ => throw new ArgumentException("Unknown spring state"),
    };
}