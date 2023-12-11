namespace day10;

public static class MapParser
{
    public static Map ParseMap(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var height = lines.Length;
        var width = lines[0].Length;
        var tiles = new Tile[height, width];
        var starts = new List<Position>();

        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                var tile = ParseTile(lines[row][column]);
                tiles[row, column] = tile;
                if (tile == Tile.Start)
                    starts.Add(new Position(column, row));
            }
        }

        var start = starts.Single();
        FixStartingTile(tiles, start);

        return new Map(tiles, start);
    }

    private static Tile ParseTile(char input) => input switch
    {
        '|' => Tile.Vertical,
        '-' => Tile.Horizontal,
        'L' => Tile.NorthEast,
        'J' => Tile.NorthWest,
        '7' => Tile.SouthWest,
        'F' => Tile.SouthEast,
        '.' => Tile.Ground,
        'S' => Tile.Start,
        _ => throw new ArgumentException("Invalid tile character"),
    };

    private static void FixStartingTile(Tile[,] tiles, Position start)
    {
        tiles[start.Y, start.X] = GetMatchingTile(
            getTile(start.GetLeft()), getTile(start.GetRight()),
            getTile(start.GetUp()), getTile(start.GetDown()));
        return;

        bool isValidPosition(Position p)
        {
            return 0 <= p.X && p.X < tiles.GetLength(1) &&
                   0 <= p.Y && p.Y < tiles.GetLength(0);
        }

        Tile getTile(Position p)
        {
            return isValidPosition(p) ? tiles[p.Y, p.X] : Tile.Ground;
        }
    }

    private static Tile GetMatchingTile(Tile leftTile, Tile rightTile, Tile upTile, Tile downTile)
    {
        var left = leftTile.CanGoRight();
        var right = rightTile.CanGoLeft();
        var up = upTile.CanGoDown();
        var down = downTile.CanGoUp();

        var potentialStartTiles = new List<Tile>();

        if (left && right)
            potentialStartTiles.Add(Tile.Horizontal);
        if (up && down)
            potentialStartTiles.Add(Tile.Vertical);
        if (up && right)
            potentialStartTiles.Add(Tile.NorthEast);
        if (up && left)
            potentialStartTiles.Add(Tile.NorthWest);
        if (down && left)
            potentialStartTiles.Add(Tile.SouthWest);
        if (down && right)
            potentialStartTiles.Add(Tile.SouthEast);

        return potentialStartTiles.Single();
    }
}