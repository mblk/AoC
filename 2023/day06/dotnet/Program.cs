using System.Diagnostics;
using aoc.common;

namespace day06;

public static class Program
{
    public static void Main(string[] args)
    {
        var lines = File.ReadAllLines(args[0]);
        Debug.Assert(lines.Length == 2);

        var times = ParseNumbers(lines[0]);
        var distances = ParseNumbers(lines[1]);
        Debug.Assert(times.Length == distances.Length);

        times.DumpToConsole();
        distances.DumpToConsole();

        var part1 = 1;
        for (var i = 0; i < times.Length; i++)
            part1 *= GetWinningScenarios(times[i], distances[i]);
        Console.WriteLine($"Part1: {part1}");
        
        var combinedTime = ParseLongNumber(lines[0]);
        var combinedDistance = ParseLongNumber(lines[1]);
        var part2 = GetWinningScenarios(combinedTime, combinedDistance);
        Console.WriteLine($"Part2: {part2}");
    }

    private static int GetWinningScenarios(long raceDuration, long winningDistance)
    {
        var wins = 0;
        for (var holdTime = 0; holdTime <= raceDuration; holdTime++)
        {
            var achievedDistance = SimulateRace(raceDuration, holdTime);
            if (achievedDistance > winningDistance)
                wins++;
        }
        return wins;
    }

    private static long SimulateRace(long raceDuration, long holdTime)
    {
        var speed = Math.Min(raceDuration, holdTime);
        var movingTime = Math.Max(0, raceDuration - holdTime);
        var achievedDistance = speed * movingTime;
        return achievedDistance;
    }
    
    private static long[] ParseNumbers(string input)
    {
        return input
            .Split(':', StringSplitOptions.TrimEntries)[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Int64.Parse)
            .ToArray();
    }

    private static long ParseLongNumber(string input)
    {
        var s = input
            .Split(':', StringSplitOptions.TrimEntries)[1]
            .Replace(" ", "");
        return Int64.Parse(s);
    }
}