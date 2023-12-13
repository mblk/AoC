namespace day12;

public enum SpringState
{
    Operational,
    Damaged,
    Unknown,
}

public record SpringRow(SpringState[] States, int[] Groups);

public static class Program
{
    public static void Main(string[] args)
    {
        var rows = ParseSpringRows(args[0]);
        var expandedRows = rows
            .Select(r => ExpandSpringRow(r, 5))
            .ToArray();
        
        var numArrangements = rows
            .Select(FindSpringRowArrangements)
            .Sum();
        var numExpandedArrangements = expandedRows
            .Select(FindSpringRowArrangements)
            .Sum();

        Console.WriteLine($"Part1: {numArrangements}");
        Console.WriteLine($"Part2: {numExpandedArrangements}");
    }

    private static long FindSpringRowArrangements(SpringRow springRow)
    {
        var states = springRow.States;
        var groups = springRow.Groups;
        var cache = new Dictionary<(int, int, int), long>();

        return solve(0, 0, 0);

        long solve(int statePos, int groupPos, int matched)
        {
            var key = (statePos, groupPos, matched);
            if (!cache.TryGetValue(key, out var count))
                cache.Add(key, count = solveInner(statePos, groupPos, matched));
            return count;
        }

        long solveInner(int statePos, int groupPos, int matched)
        {
            if (statePos == states.Length)
                return groupPos == groups.Length && matched == 0 ||
                       groupPos == groups.Length - 1 && matched == groups.Last()
                    ? 1
                    : 0;

            return states[statePos] switch
            {
                SpringState.Operational => operationalCase(),
                SpringState.Damaged => damagedCase(),
                _ => operationalCase() + damagedCase(),
            };

            long operationalCase()
            {
                if (matched == 0)
                    return solve(statePos + 1, groupPos, 0);
                if (groupPos < groups.Length && groups[groupPos] == matched)
                    return solve(statePos + 1, groupPos + 1, 0);
                return 0;
            }

            long damagedCase()
            {
                return solve(statePos + 1, groupPos, matched + 1);
            }
        }
    }

    private static SpringRow[] ParseSpringRows(string filename)
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

    private static SpringRow ExpandSpringRow(SpringRow row, int count)
    {
        var states = new List<SpringState>();
        var groups = new List<int>();

        for (var i = 0; i < count; i++)
        {
            states.AddRange(row.States);

            if (i < count - 1)
                states.Add(SpringState.Unknown);

            groups.AddRange(row.Groups);
        }

        return new SpringRow(states.ToArray(), groups.ToArray());
    }
}