using System.Text.Json;

namespace aoc.common;

public static class ObjectExtensions
{
    public static T DumpToConsole<T>(this T obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        });
        Console.WriteLine(json);
        return obj;
    }
}