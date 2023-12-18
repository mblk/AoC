using aoc.common.Maps;
using Map = aoc.common.Maps.Map<int>;

namespace day17;

public static class Program
{
    public static void Main(string[] args)
    {
        var map = MapParser.ParseIntegerMap<int>(args[0]);

        var start = new Position(0, 0);
        var goal = new Position(map.Width - 1, map.Height - 1);

        var path1 = new DijkstraPathFinder(map, 0, 3)
            .FindPath(start, goal);
        var path2 = new DijkstraPathFinder(map, 4, 10)
            .FindPath(start, goal);

        //PrintPath(map, path1);
        //PrintPath(map, path2);

        Console.WriteLine($"Part1: {CalculateHeatLoss(map, path1)}");
        Console.WriteLine($"Part2: {CalculateHeatLoss(map, path2)}");
    }

    private static int CalculateHeatLoss(Map map, IEnumerable<Position> path)
    {
        return path.Skip(1).Sum(p => map.Get(p.X, p.Y));
    }

    private static void PrintPath(Map map, Position[] path)
    {
        Console.WriteLine("== path ==");

        for (var y = 0; y < map.Height; y++)
        {
            var s = "";
            for (var x = 0; x < map.Width; x++)
            {
                var isPath = path.Contains(new Position(x, y));
                s += isPath ? '#' : '.';
            }

            Console.WriteLine(s);
        }
    }
}