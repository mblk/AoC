namespace day10;

public static class TileExtensions
{
    public static bool CanGoLeft(this Tile tile)
        => tile is Tile.Start or Tile.Horizontal or Tile.NorthWest or Tile.SouthWest;

    public static bool CanGoRight(this Tile tile)
        => tile is Tile.Start or Tile.Horizontal or Tile.NorthEast or Tile.SouthEast;

    public static bool CanGoUp(this Tile tile)
        => tile is Tile.Start or Tile.Vertical or Tile.NorthEast or Tile.NorthWest;

    public static bool CanGoDown(this Tile tile)
        => tile is Tile.Start or Tile.Vertical or Tile.SouthEast or Tile.SouthWest;
}