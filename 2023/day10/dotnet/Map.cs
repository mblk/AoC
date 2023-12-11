namespace day10;

public class Map
{
    private readonly Tile[,] _tiles;
    
    public int Width { get; }
    public int Height { get; }
    public Position Start { get; }

    public Map(Tile[,] tiles, Position start)
    {
        // indexing: array[y, x]
        
        Height = tiles.GetLength(0);
        Width = tiles.GetLength(1);
        
        _tiles = tiles;
        Start = start;
    }
    
    public Tile Get(Position p)
    {
        return IsValidPosition(p)
            ? _tiles[p.Y, p.X]
            : Tile.Ground;
    }

    public bool IsValidPosition(Position p)
    {
        return 0 <= p.X && p.X < Width &&
               0 <= p.Y && p.Y < Height;
    }
}