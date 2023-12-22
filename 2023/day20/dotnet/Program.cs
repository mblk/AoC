using System.Diagnostics;
using aoc.common;

namespace day20;

public static class Program
{
    public static void Main(string[] args)
    {
        var modules = ModuleParser.ParseModules(args[0]);
        
        {
            var (numLow, numHigh) = new PulseSimulator(modules).SimulateAndCountPulses(1000);
            var part1 = (long)numLow * (long)numHigh;

            Console.WriteLine($"Part1: {part1}");
            Debug.Assert(part1 == 925955316L);
        }

        {
            var c1 = new PulseSimulator(modules).SimulateUntilHighPulse("qz");
            var c2 = new PulseSimulator(modules).SimulateUntilHighPulse("cq");
            var c3 = new PulseSimulator(modules).SimulateUntilHighPulse("jx");
            var c4 = new PulseSimulator(modules).SimulateUntilHighPulse("tt");

            var part2 = MathUtils.LeastCommonMultiple(new long[] { c1, c2, c3, c4 });
            Console.WriteLine($"Part1: {part2}");
            Debug.Assert(part2 == 241528477694627L);
        }
    }
}