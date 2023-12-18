namespace day17;

public readonly record struct Position(int X, int Y)
{
    public Position Move(Direction direction)
    {
        var (dx, dy) = direction.GetDelta();
        return new Position(X + dx, Y + dy);
    }
    
    public override string ToString()
    {
        return $"[{X},{Y}]";
    }
}