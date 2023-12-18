using System.Diagnostics;

namespace day17;

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}

public static class DirectionExtensions
{
    public static (int, int) GetDelta(this Direction direction) => direction switch
    {
        Direction.Right => (1, 0),
        Direction.Left => (-1, 0),
        Direction.Down => (0, 1),
        Direction.Up => (0, -1),
        _ => throw new UnreachableException(),
    };

    public static Direction TurnLeft(this Direction direction) => direction switch
    {
        Direction.Right => Direction.Up,
        Direction.Up => Direction.Left,
        Direction.Left => Direction.Down,
        Direction.Down => Direction.Right,
        _ => throw new UnreachableException(),
    };

    public static Direction TurnRight(this Direction direction) => direction switch
    {
        Direction.Right => Direction.Down,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        Direction.Up => Direction.Right,
        _ => throw new UnreachableException(),
    };
}