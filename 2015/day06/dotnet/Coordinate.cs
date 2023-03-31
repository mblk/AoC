namespace day06;

internal readonly struct Coordinate
{
    public int X { get; }
    public int Y { get; }

    private Coordinate(int x, int y)
        => (X, Y) = (x, y);

    public override string ToString()
        => $"({X},{Y})";
    
    public static Coordinate Parse(string input, int fieldDimension)
    {
        string[] parts = input.Split(',');
        if (parts.Length != 2)
            throw new ArgumentException($"Can't parse coordinate '{input}'");

        if (!Int32.TryParse(parts[0], out int x) ||
            !Int32.TryParse(parts[1], out int y) ||
            x < 0 ||
            y < 0 ||
            x >= fieldDimension ||
            y >= fieldDimension)
            throw new ArgumentException($"Can't parse coordinate '{input}'");

        return new Coordinate(x, y);
    }
}
