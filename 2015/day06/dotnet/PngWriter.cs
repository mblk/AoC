using BigGustave;

namespace day06;

internal static class PngWriter
{
    public static void WriteBlackAndWhite(int[,] data, string filename)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        
        var builder = PngBuilder.Create(width, height, hasAlphaChannel: false);

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            if (data[x, y] > 0)
                builder.SetPixel(255, 255, 255, x, y);

        using var memory = new MemoryStream();
        builder.Save(memory);
        File.WriteAllBytes(filename, memory.ToArray());
    }
    
    public static void WriteGrayscale(int[,] data, string filename)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        
        var builder = PngBuilder.Create(width, height, hasAlphaChannel: false);

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var gray = (byte)Math.Min(255, data[x, y]);
            builder.SetPixel(gray, gray, gray, x, y);
        }

        using var memory = new MemoryStream();
        builder.Save(memory);
        File.WriteAllBytes(filename, memory.ToArray());
    }
}
