namespace day09;

public static class Program
{
    public static void Main(string[] args)
    {
        var data = ParseData(args[0]);

        var forwardExtrapolatedValues = data
            .Select(v => Extrapolate(v, true))
            .ToArray();
        var backwardExtrapolatedValues = data
            .Select(v => Extrapolate(v, false))
            .ToArray();

        Console.WriteLine($"Part1: {forwardExtrapolatedValues.Sum()}");
        Console.WriteLine($"Part2: {backwardExtrapolatedValues.Sum()}");
    }

    private static int Extrapolate(int[] values, bool forward)
    {
        var table = new List<int[]> { values };

        while (true)
        {
            var diffs = GetDiffs(table.Last());
            if (diffs.Sum() == 0)
                break;
            table.Add(diffs);
        }

        return table.AsEnumerable()
            .Reverse()
            .Aggregate(0, aggr);

        int aggr(int current, int[] row) => forward
            ? row.Last() + current
            : row.First() - current;
    }

    private static int[] GetDiffs(int[] values)
    {
        var diffs = new int[values.Length - 1];
        for (int i = 0; i < values.Length - 1; i++)
            diffs[i] = values[i + 1] - values[i];
        return diffs;
    }

    private static int[][] ParseData(string filename)
    {
        return File.ReadAllLines(filename)
            .Select(ParseLine)
            .ToArray();
    }

    private static int[] ParseLine(string input)
    {
        return input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Int32.Parse)
            .ToArray();
    }
}