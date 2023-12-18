using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace aoc.common.Maps;

public static class MapParser
{
    public static Map<T> ParseMap<T>(string filename, Func<char, T> tileParser)
    {
        var lines = File.ReadAllLines(filename)
            .Where(s => !String.IsNullOrWhiteSpace(s))
            .ToArray();

        var height = lines.Length;
        if (height < 1)
            throw new InvalidOperationException("Height must be > 0");

        var width = lines[0].Length;
        if (width < 1)
            throw new InvalidOperationException("Width must be > 0");

        if (lines.Any(s => s.Length != width))
            throw new InvalidOperationException("Lines have different lengths");

        var tiles = new T[height, width];

        for (var row = 0; row < height; row++)
        for (var column = 0; column < width; column++)
            tiles[row, column] = tileParser(lines[row][column]);

        return new Map<T>(tiles);
    }

    public static Map<T> ParseIntegerMap<T>(string filename)
        where T : IBinaryInteger<T>
    {
        return ParseMap(filename, parseFunc);

        T parseFunc(char c)
        {
            return T.Parse(new[] { c }, CultureInfo.InvariantCulture);
        }
    }

    public static Map<T> ParseEnumMap<T>(string filename)
        where T : struct, Enum
    {
        var charToEnumMapping = GetCharacterToEnumMapping<T>();

        return ParseMap(filename, parseFunc);

        T parseFunc(char c)
        {
            return charToEnumMapping.TryGetValue(c, out var value)
                ? value
                : throw new InvalidOperationException($"Found character without mapping: {c.ToString()}");
        }
    }

    private static IReadOnlyDictionary<char, T> GetCharacterToEnumMapping<T>()
        where T : struct, Enum
    {
        var charToEnumMapping = new Dictionary<char, T>();

        var fields = typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<EnumMapCharacterAttribute>() ??
                       throw new InvalidOperationException($"Missing attribute on: {field.Name}");
            var value = (T)field.GetValue(null)!;
            charToEnumMapping.Add(attr.Character, value);
        }

        return charToEnumMapping;
    }
}