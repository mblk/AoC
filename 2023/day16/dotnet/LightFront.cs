using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace day16;

public readonly record struct Position(int X, int Y);

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}

public readonly record struct LightFront(Position Position, Direction Direction)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Move(Tile tile, LightFront[] result)
    {
        switch (tile)
        {
            case Tile.SlashMirror:
                result[0] = Move(ReflectSlashMirror(Direction));
                return 1;

            case Tile.BackslashMirror:
                result[0] = Move(ReflectBackslashMirror(Direction));
                return 1;

            case Tile.HorizontalSplitter when Direction is Direction.Up or Direction.Down:
                result[0] = Move(Direction.Left);
                result[1] = Move(Direction.Right);
                return 2;

            case Tile.VerticalSplitter when Direction is Direction.Left or Direction.Right:
                result[0] = Move(Direction.Up);
                result[1] = Move(Direction.Down);
                return 2;

            default:
                result[0] = Move(Direction);
                return 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LightFront Move(Direction direction)
    {
        var (x, y) = (Position.X, Position.Y);
        var (dx, dy) = GetDirectionDelta(direction);
        var p = new Position(x + dx, y + dy);
        return new LightFront(p, direction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int, int) GetDirectionDelta(Direction direction) => direction switch
    {
        Direction.Right => (1, 0),
        Direction.Left => (-1, 0),
        Direction.Down => (0, 1),
        Direction.Up => (0, -1),
        _ => throw new UnreachableException(),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private static Direction ReflectSlashMirror(Direction direction) => direction switch
    {
        Direction.Right => Direction.Up,
        Direction.Left => Direction.Down,
        Direction.Down => Direction.Left,
        Direction.Up => Direction.Right,
        _ => throw new UnreachableException()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Direction ReflectBackslashMirror(Direction direction) => direction switch
    {
        Direction.Right => Direction.Down,
        Direction.Left => Direction.Up,
        Direction.Down => Direction.Right,
        Direction.Up => Direction.Left,
        _ => throw new UnreachableException()
    };
}