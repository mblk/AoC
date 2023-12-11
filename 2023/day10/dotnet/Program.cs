using aoc.common;

namespace day10;

public static class Program
{
    public static void Main(string[] args)
    {
        var map = MapParser.ParseMap(args[0]);

        var loop = FindLoop(map);
        var enclosedTiles = GetEnclosedTiles(map, loop);

        Console.WriteLine($"Part1: {loop.Values.Max()}");
        Console.WriteLine($"Part2: {enclosedTiles.Count()}");
    }

    private static IReadOnlyDictionary<Position, int> FindLoop(Map map)
    {
        var loop = new Dictionary<Position, int>();
        var heads = new[] { map.Start };
        var distanceToStart = 0;

        while (true)
        {
            foreach (var p in heads)
                loop.Add(p, distanceToStart);

            var next = heads
                .SelectMany(p => GetConnectedTiles(map, p))
                .Distinct()
                .Where(p => !loop.ContainsKey(p))
                .ToArray();

            if (!next.Any()) break;

            heads = next;
            distanceToStart++;
        }

        return loop;
    }

    private static IEnumerable<Position> GetConnectedTiles(Map map, Position p)
    {
        var left = p.GetLeft();
        var right = p.GetRight();
        var up = p.GetUp();
        var down = p.GetDown();

        var thisTile = map.Get(p);
        var leftTile = map.Get(left);
        var rightTile = map.Get(right);
        var upTile = map.Get(up);
        var downTile = map.Get(down);

        if (thisTile.CanGoLeft() && leftTile.CanGoRight())
            yield return left;
        if (thisTile.CanGoRight() && rightTile.CanGoLeft())
            yield return right;
        if (thisTile.CanGoUp() && upTile.CanGoDown())
            yield return up;
        if (thisTile.CanGoDown() && downTile.CanGoUp())
            yield return down;
    }

    private static IEnumerable<Position> GetEnclosedTiles(Map map, IReadOnlyDictionary<Position, int> loop)
    {
        var enclosedTiles = new List<Position>();

        for (var startX = 0; startX < map.Width; startX++)
        {
            var topLeft = new Position(startX, 0);
            var isInside = false;
            foreach (var p in ShootDiagonalRay(map, topLeft))
            {
                var isLoopPart = loop.ContainsKey(p);
                if (isLoopPart)
                {
                    if (ToggleInsideOutside(map.Get(p)))
                        isInside = !isInside;
                }
                else
                {
                    if (isInside)
                        enclosedTiles.Add(p);
                }
            }
        }

        for (var startY = 1; startY < map.Height; startY++)
        {
            var topLeft = new Position(0, startY);
            var isInside = false;
            foreach (var p in ShootDiagonalRay(map, topLeft))
            {
                var isLoopPart = loop.ContainsKey(p);
                if (isLoopPart)
                {
                    if (ToggleInsideOutside(map.Get(p)))
                        isInside = !isInside;
                }
                else
                {
                    if (isInside)
                        enclosedTiles.Add(p);
                }
            }
        }

        return enclosedTiles;
    }

    private static bool ToggleInsideOutside(Tile tile) =>
        tile is Tile.Horizontal or Tile.Vertical or Tile.NorthWest or Tile.SouthEast;

    private static IEnumerable<Position> ShootDiagonalRay(Map map, Position topLeft)
    {
        var offset = 0;
        while (true)
        {
            var p = new Position(topLeft.X + offset, topLeft.Y + offset);
            offset++;
            if (!map.IsValidPosition(p)) break;
            yield return p;
        }
    }
}