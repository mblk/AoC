namespace day05;

public record Config(long[] Seeds, Range[] SeedRanges, Mapping[] Mappings);

public record Range(long Start, long Length)
{
    public long End { get; } = Start + Length - 1;

    public bool Contains(long value)
    {
        return Start <= value && value <= End;
    }

    public override string ToString()
    {
        return $"[{Start}..{End}]";
    }
}

public record Mapping(MappingEntry[] Entries);

public record MappingEntry(long DestinationStart, long SourceStart, long Length)
{
    public long DestinationEnd { get; } = DestinationStart + Length - 1;
    public long SourceEnd { get; } = SourceStart + Length - 1;

    public override string ToString()
    {
        return $"[{SourceStart}..{SourceEnd}] -> [{DestinationStart}..{DestinationEnd}]";
    }
}
