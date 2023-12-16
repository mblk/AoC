using System.Runtime.CompilerServices;

namespace day16;

public enum Tile
{
    Ground,
    SlashMirror,
    BackslashMirror,
    VerticalSplitter,
    HorizontalSplitter,
}

public class Map
{
    private readonly Tile[,] _tiles;
    
    public int Height { get; }
    public int Width { get; }

    public Map(Tile[,] tiles)
    {
        _tiles = tiles;
        Height = tiles.GetLength(0);
        Width = tiles.GetLength(1);
    }
   
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tile Get(Position position)
    {
        return _tiles[position.Y, position.X];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Position position)
    {
        return 0 <= position.X && position.X < Width &&
               0 <= position.Y && position.Y < Height;
    }
}