namespace day20;

public class Module
{
    public required string Name { get; init; }
    public required ModuleType Type { get; init; }
    public required IReadOnlyList<string> OutputModules { get; init; }
    public required IReadOnlyList<string> InputModules { get; set; }
}

public enum ModuleType
{
    Broadcaster,
    FlipFlop,
    Conjunction,
    Dummy,
}