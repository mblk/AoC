using aoc.common;

namespace day11;

public readonly record struct Position(long X, long Y)
{
    public long DistanceTo(Position other)
    {
        // Only move left, right, up, down.
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"Part1: {GetSumOfDistancesBetweenGalaxies(args[0], 1)}");
        Console.WriteLine($"Part2: {GetSumOfDistancesBetweenGalaxies(args[0], 1_000_000 - 1)}");
    }

    private static long GetSumOfDistancesBetweenGalaxies(string filename, int expansionDistance)
    {
        return ParseGalaxies(filename, expansionDistance)
            .GetCombinationPairs()
            .Select(p => p.Item1.DistanceTo(p.Item2))
            .Sum();
    }

    private static IReadOnlyList<Position> ParseGalaxies(string filename, long expansionDistance)
    {
        const char galaxySymbol = '#';

        var lines = File.ReadAllLines(filename);
        var height = lines.Length;
        var width = lines[0].Length;

        var emptyRows = Enumerable.Range(0, height)
            .Where(row => !lines[row].Contains(galaxySymbol))
            .ToArray();

        var emptyColumns = Enumerable.Range(0, width)
            .Where(column => Enumerable.Range(0, height)
                .Select(row => lines[row][column])
                .All(c => c != galaxySymbol)
            )
            .ToArray();

        var galaxies = new List<Position>();
        var effectiveY = 0L;
        for (var row = 0; row < height; row++, effectiveY++)
        {
            var effectiveX = 0L;
            for (var column = 0; column < width; column++, effectiveX++)
            {
                if (lines[row][column] == galaxySymbol)
                    galaxies.Add(new Position(effectiveX, effectiveY));
                
                if (emptyColumns.Contains(column))
                    effectiveX += expansionDistance;
            }

            if (emptyRows.Contains(row))
                effectiveY += expansionDistance;
        }

        return galaxies;
    }
}