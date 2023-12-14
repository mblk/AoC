namespace day14;

public static class MapParser
{
    public static Map ParseMap(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var height = lines.Length;
        var width = lines[0].Length;

        var tiles = new Tile[height, width];

        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
            tiles[row, col] = ParseTile(lines[row][col]);

        return new Map(tiles);
    }

    private static Tile ParseTile(char input) => input switch
    {
        'O' => Tile.LooseRock,
        '#' => Tile.FixedRock,
        '.' => Tile.Ground,
        _ => throw new ArgumentException("Invalid tile character"),
    };
}