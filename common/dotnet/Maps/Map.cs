namespace aoc.common.Maps;

public class Map<T>
{
    private readonly T[,] _tiles;

    public int Height { get; }
    public int Width { get; }

    public Map(T[,] tiles)
    {
        _tiles = tiles;

        Height = tiles.GetLength(0);
        Width = tiles.GetLength(1);

        if (Height < 1) throw new ArgumentException("Height must be > 0");
        if (Width < 1) throw new ArgumentException("Width must be > 0");
    }

    public T Get(int x, int y)
    {
        return _tiles[y, x];
    }

    public bool Contains(int x, int y)
    {
        return 0 <= x && x < Width &&
               0 <= y && y < Height;
    }
}