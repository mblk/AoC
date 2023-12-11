using System.Runtime.CompilerServices;
using System.Text.Json;

namespace aoc.common;

public static class ObjectExtensions
{
    public static T DumpToConsole<T>(this T obj, string? header = null,
        [CallerArgumentExpression(nameof(obj))] string? expr = null)
    {
        Console.WriteLine($"----- {expr} ({header}) -----");
        
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        });
        Console.WriteLine(json);
        return obj;
    }
}