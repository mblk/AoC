namespace day10;

public readonly record struct Position(int X, int Y)
{
    public Position GetLeft() => this with { X = X - 1 };

    public Position GetRight() => this with { X = X + 1 };

    public Position GetUp() => this with { Y = Y - 1 };

    public Position GetDown() => this with { Y = Y + 1 };
}