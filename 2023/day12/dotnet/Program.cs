using aoc.common;

namespace day12;

public static class Program
{
    public static void Main(string[] args)
    {
        SpringRow[] rows;
        //using (new TimedBlock("Parse"))
        {
            rows = Parser.ParseSpringRows(args[0]);
        }

        SpringRow[] expandedRows;
        //using (new TimedBlock("Expand"))
        {
            expandedRows = rows
                .Select(r => r.Expand(5))
                .ToArray();
        }

        long numArrangements;
        //using (new TimedBlock("Find1"))
        {
            numArrangements = rows
                .Select(ArrangementFinder.FindArrangements)
                .Sum();
        }

        long numExpandedArrangements;
        //using (new TimedBlock("Find2"))
        {
            numExpandedArrangements = expandedRows
                .Select(ArrangementFinder.FindArrangements)
                .Sum();
        }

        Console.WriteLine($"Part1: {numArrangements}");
        Console.WriteLine($"Part2: {numExpandedArrangements}");
    }
}