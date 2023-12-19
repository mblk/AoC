using System.Diagnostics;

namespace day18;

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}

public record Instruction(Direction Direction, long Length, string Color);

public record Coord(long X, long Y);

public static class Program
{
    public static void Main(string[] args)
    {
        var instructions = ParseInstructions(args[0]);
        var coords = GetCoords(instructions);
        var area = CalculateArea(coords);

        var fixedInstructions = FixInstructions(instructions);
        var fixedCoords = GetCoords(fixedInstructions);
        var fixedArea = CalculateArea(fixedCoords);

        Console.WriteLine($"Part1: {area}");
        Console.WriteLine($"Part2: {fixedArea}");
    }

    private static long CalculateArea(IReadOnlyList<Coord> coords)
    {
        var area = 0L;

        for (var i = 0; i < coords.Count; i++)
        {
            var c1 = coords[i];
            var c2 = coords[(i + 1) % coords.Count];

            // Trapezoid formula
            area += (c1.Y + c2.Y) * (c1.X - c2.X);
        }

        return area / 2;
    }

    private static IReadOnlyList<Coord> GetCoords(IReadOnlyList<Instruction> instructions)
    {
        var coords = new List<Coord>(instructions.Count);

        var x = 0L;
        var y = 0L;

        for (var i = 0; i < instructions.Count; i++)
        {
            var (dir, length, _) = instructions[i];
            var (nextDir, _, _) = instructions[(i + 1) % instructions.Count];

            var (dx, dy) = GetDelta(dir);
            x += dx * length;
            y += dy * length;

            coords.Add(GetOuterEdge(x, y, dir, nextDir));
        }

        return coords;
    }

    private static (long, long) GetDelta(Direction direction) => direction switch
    {
        Direction.Right => (1, 0),
        Direction.Left => (-1, 0),
        Direction.Down => (0, 1),
        Direction.Up => (0, -1),
        _ => throw new UnreachableException(),
    };

    private static Coord GetOuterEdge(long x, long y, Direction prevDir, Direction nextDir)
    {
        if (prevDir == Direction.Down || nextDir == Direction.Down)
            x++;

        if (prevDir == Direction.Left || nextDir == Direction.Left)
            y++;

        return new Coord(x, y);
    }

    private static Instruction[] ParseInstructions(string filename)
    {
        return File.ReadAllLines(filename)
            .Select(ParseInstruction)
            .ToArray();
    }

    private static Instruction ParseInstruction(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.TrimEntries);

        var direction = ParseDirection(parts[0]);
        var length = Int32.Parse(parts[1]);
        var color = parts[2][2..^1];

        return new Instruction(direction, length, color);
    }

    private static Direction ParseDirection(string input) => input switch
    {
        "U" => Direction.Up,
        "R" => Direction.Right,
        "D" => Direction.Down,
        "L" => Direction.Left,
        _ => throw new ArgumentException("Invalid direction"),
    };

    private static Instruction[] FixInstructions(IEnumerable<Instruction> instructions)
    {
        return instructions.Select(FixInstruction).ToArray();
    }

    private static Instruction FixInstruction(Instruction instruction)
    {
        var newLength = Convert.ToInt32(instruction.Color[0..^1], 16);

        var newDir = instruction.Color.Last() switch
        {
            '0' => Direction.Right,
            '1' => Direction.Down,
            '2' => Direction.Left,
            '3' => Direction.Up,
            _ => throw new ArgumentException("Invalid direction"),
        };

        return new Instruction(newDir, newLength, "");
    }
}