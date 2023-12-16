namespace day16;

public static class MapParser
{
    public static Map ParseMap(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var height = lines.Length;
        var width = lines[0].Length;
        var tiles = new Tile[height, width];

        for (var row = 0; row < height; row++)
        for (var column = 0; column < width; column++)
            tiles[row, column] = ParseTile(lines[row][column]);
        
        return new Map(tiles);
    }

    private static Tile ParseTile(char input) => input switch
    {
        '.' => Tile.Ground,
        '/' => Tile.SlashMirror,
        '\\' => Tile.BackslashMirror,
        '|' => Tile.VerticalSplitter,
        '-' => Tile.HorizontalSplitter,
        _ => throw new ArgumentException("Invalid tile character"),
    };
}