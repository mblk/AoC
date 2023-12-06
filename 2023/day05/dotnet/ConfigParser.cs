namespace day05;

public class ConfigParser
{
    private readonly string _filename;

    public ConfigParser(string filename)
    {
        _filename = filename;
    }

    public Config Parse()
    {
        var seeds = Array.Empty<long>();
        var mappings = new List<Mapping>();
        var bufferedEntries = new List<MappingEntry>();

        void flushBuffer()
        {
            if (!bufferedEntries.Any()) return;
            mappings.Add(new Mapping(bufferedEntries.ToArray()));
            bufferedEntries.Clear();
        }

        foreach (var line in File.ReadAllLines(_filename))
        {
            if (String.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("seeds:"))
            {
                seeds = line[6..]
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse)
                    .ToArray();
                continue;
            }

            if (line.EndsWith("map:"))
            {
                flushBuffer();
                continue;
            }

            bufferedEntries.Add(ParseMappingEntry(line));
        }

        flushBuffer();

        return new Config(seeds, CreateSeedRanges(seeds), mappings.ToArray());
    }

    private static MappingEntry ParseMappingEntry(string input)
    {
        var numbers = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Int64.Parse)
            .ToArray();
        if (numbers.Length != 3) throw new ArgumentException($"Invalid mapping entry: {input}");

        return new MappingEntry(numbers[0], numbers[1], numbers[2]);
    }

    private static Range[] CreateSeedRanges(long[] seeds)
    {
        var seedRanges = new List<Range>();
        for (var i = 0; i < seeds.Length; i += 2)
        {
            seedRanges.Add(new Range(seeds[i], seeds[i + 1]));
        }
        return seedRanges.ToArray();
    }
}