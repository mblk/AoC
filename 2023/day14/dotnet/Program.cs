using System.Diagnostics;
using aoc.common;

namespace day14;

public static class Program
{
    public static void Main(string[] args)
    {
        var map = MapParser.ParseMap(args[0]);

        var part1 = map.Tilt(TiltDirection.North)
            .CalculateWeight();

        var part2 = CycleMapForAVeryLongTime(map)
            .CalculateWeight();

        Console.WriteLine($"Part1: {part1}");
        Console.WriteLine($"Part2: {part2}");
    }

    private static Map CycleMapForAVeryLongTime(Map map)
    {
        // Assumption: Map state follows a repeating cycle after some ticks.

        const int totalTicksToSimulate = 1_000_000_000;

        var history = new List<Map>();

        for (var tick = 0; tick < totalTicksToSimulate; tick++)
        {
            map = map.TiltFullCircle();

            // Already saw this state of the map before?
            var idx = history.IndexOf(map);
            if (idx != -1)
            {
                var cycleLength = tick - idx;
                var firstCycleBegin = tick - cycleLength;
                var totalRepeatingTicks = totalTicksToSimulate - firstCycleBegin;
                var finalIndex = firstCycleBegin + (totalRepeatingTicks % cycleLength) - 1;
                var finalMap = history[finalIndex];

                return finalMap;
            }

            history.Add(map);
        }

        throw new UnreachableException();
    }
}