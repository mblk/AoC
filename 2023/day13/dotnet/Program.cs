using System.Diagnostics;

namespace day13;

public record Box(int Width, int Height, bool[][] Data);

public enum MirrorDirection
{
    Horizontal,
    Vertical
};

public record Mirror(MirrorDirection Direction, int Position);

public static class Program
{
    public static void Main(string[] args)
    {
        var boxes = ParseInput(args[0])
            .ToArray();

        var score1 = boxes
            .Select(FindMirror)
            .Select(CalculateScore)
            .Sum();

        var score2 = boxes
            .Select(FindSmudgedMirror)
            .Select(CalculateScore)
            .Sum();

        Console.WriteLine($"Part1: {score1}");
        Console.WriteLine($"Part2: {score2}");
    }

    private static Mirror FindMirror(Box box)
    {
        for (var row = 1; row < box.Height; row++)
            if (IsHorizontalMirrorAxis(box, row))
                return new Mirror(MirrorDirection.Horizontal, row);

        for (var column = 1; column < box.Width; column++)
            if (IsVerticalMirrorAxis(box, column))
                return new Mirror(MirrorDirection.Vertical, column);

        throw new UnreachableException();
    }

    private static Mirror FindSmudgedMirror(Box box)
    {
        var oldMirror = FindMirror(box);

        for (var smudgeRow = 0; smudgeRow < box.Height; smudgeRow++)
        {
            for (var smudgeColumn = 0; smudgeColumn < box.Width; smudgeColumn++)
            {
                box.Data[smudgeRow][smudgeColumn] ^= true;

                for (var row = 1; row < box.Height; row++)
                {
                    if (oldMirror.Direction == MirrorDirection.Horizontal && oldMirror.Position == row)
                        continue;
                    if (IsHorizontalMirrorAxis(box, row))
                        return new Mirror(MirrorDirection.Horizontal, row);
                }

                for (var column = 1; column < box.Width; column++)
                {
                    if (oldMirror.Direction == MirrorDirection.Vertical && oldMirror.Position == column)
                        continue;
                    if (IsVerticalMirrorAxis(box, column))
                        return new Mirror(MirrorDirection.Vertical, column);
                }

                box.Data[smudgeRow][smudgeColumn] ^= true;
            }
        }

        throw new UnreachableException();
    }

    private static bool IsHorizontalMirrorAxis(Box box, int position)
    {
        Debug.Assert(position > 0);
        Debug.Assert(position < box.Height);

        for (int upper = position - 1, lower = position;
             upper >= 0 && lower < box.Height;
             upper--, lower++)
        {
            var upperRow = box.Data[upper];
            var lowerRow = box.Data[lower];

            if (!upperRow.SequenceEqual(lowerRow))
                return false;
        }

        return true;
    }

    private static bool IsVerticalMirrorAxis(Box box, int position)
    {
        Debug.Assert(position > 0);
        Debug.Assert(position < box.Width);

        for (int left = position - 1, right = position;
             left >= 0 && right < box.Width;
             left--, right++)
        {
            var leftColumn = Enumerable.Range(0, box.Height)
                .Select(row => box.Data[row][left]);
            var rightColumn = Enumerable.Range(0, box.Height)
                .Select(row => box.Data[row][right]);

            if (!leftColumn.SequenceEqual(rightColumn))
                return false;
        }

        return true;
    }

    private static int CalculateScore(Mirror axis)
        => axis.Direction == MirrorDirection.Horizontal
            ? axis.Position * 100
            : axis.Position;

    private static IEnumerable<Box> ParseInput(string filename)
    {
        var buffer = new List<string>();

        foreach (var line in File.ReadLines(filename))
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                yield return ParseBox(buffer);
                buffer.Clear();
            }
            else
            {
                buffer.Add(line);
            }
        }

        if (buffer.Any())
        {
            yield return ParseBox(buffer);
            buffer.Clear();
        }
    }

    private static Box ParseBox(IReadOnlyList<string> lines)
    {
        var height = lines.Count;
        var width = lines[0].Length;
        var data = lines
            .Select(ParseBoxLine)
            .ToArray();
        return new Box(width, height, data);
    }

    private static bool[] ParseBoxLine(string input) => input
        .ToCharArray()
        .Select(c => c == '#')
        .ToArray();
}