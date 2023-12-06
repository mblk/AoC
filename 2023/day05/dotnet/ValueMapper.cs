namespace day05;

public class ValueMapper
{
    private readonly Mapping[] _mappings;

    public ValueMapper(Mapping[] mappings)
    {
        _mappings = mappings;
    }

    public long MapValue(long startingValue)
    {
        return _mappings.Aggregate(startingValue, (current, mapping) => ApplyMappingToValue(mapping, current));
    }

    public Range[] MapRange(Range startingRange)
    {
        return _mappings.Aggregate(new[] { startingRange }, (current, mapping) =>
            current.SelectMany(r => ApplyMappingToRange(mapping, r))
                .ToArray());
    }

    private static long ApplyMappingToValue(Mapping mapping, long value)
    {
        var entry = mapping.Entries.SingleOrDefault(e =>
            e.SourceStart <= value && value < e.SourceStart + e.Length);

        if (entry != null)
        {
            var delta = value - entry.SourceStart;
            return entry.DestinationStart + delta;
        }
        else
        {
            return value;
        }
    }

    private static IEnumerable<Range> ApplyMappingToRange(Mapping mapping, Range range)
    {
        return SplitRangeForMapping(mapping, range)
            .Select(mapRange);

        Range mapRange(Range r)
        {
            var mappedStart = ApplyMappingToValue(mapping, r.Start);
            return new Range(mappedStart, r.Length);
        }
    }

    private static IEnumerable<Range> SplitRangeForMapping(Mapping mapping, Range range)
    {
        var splitPositions = new List<long>
        {
            range.Start,
            range.End + 1
        };

        foreach (var e in mapping.Entries)
        {
            if (range.Contains(e.SourceStart))
                splitPositions.Add(e.SourceStart);
            if (range.Contains(e.SourceEnd + 1))
                splitPositions.Add(e.SourceEnd + 1);
        }

        var sortedSplitPositions = splitPositions
            .Distinct()
            .OrderBy(c => c)
            .ToArray();

        for (var i = 0; i < sortedSplitPositions.Length - 1; i++)
        {
            var p1 = sortedSplitPositions[i];
            var p2 = sortedSplitPositions[i + 1];
            yield return new Range(p1, p2 - p1);
        }
    }
}