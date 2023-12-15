namespace day15;

public static class Program
{
    public static void Main(string[] args)
    {
        var part1 = Parser.ParseRawInitSequence(args[0])
            .Select(HolidayStringHelper.Hash)
            .Select(h => (int)h)
            .Sum();

        Console.WriteLine($"Part1: {part1}");

        var initSequence = Parser.ParseInitSequence(args[0]);
        var state = new State();
        foreach (var step in initSequence)
            step.Execute(state);

        Console.WriteLine($"Part2: {state.CalculateFocusPower()}");
    }
}