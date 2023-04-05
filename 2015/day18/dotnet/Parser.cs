namespace day18;

internal static class Parser
{
    public static Grid Parse(string filename, bool cornerAlwaysOn = false)
    {
        int width = 0;
        int height = 0;
        
        foreach (string line in File.ReadLines(filename))
        {
            width = Math.Max(width, line.Length);
            height++;
        }
        
        Console.WriteLine($"Grid size: {width}x{height}");
        
        var grid = new Grid(width, height);
        int y = 0;
        foreach (string line in File.ReadLines(filename))
        {
            for (int x = 0; x < line.Length; x++)
            {
                bool alwaysOn = cornerAlwaysOn &&
                                (x == 0 || x == width - 1) &&
                                (y == 0 || y == height - 1);

                grid.Lights[x, y] = new Light(alwaysOn)
                {
                    State = line[x] == '#',
                };
            }
            y++;
        }

        return grid;
    }
}