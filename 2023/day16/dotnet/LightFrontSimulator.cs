namespace day16;

public static class LightFrontSimulator
{
    public static int FindMaxEnergized(Map map)
    {
        var maxEnergized = 0;

        for (var y = 0; y < map.Height; y++)
        {
            var f1 = new LightFront(new Position(0, y), Direction.Right);
            var f2 = new LightFront(new Position(map.Width - 1, y), Direction.Left);

            maxEnergized = Math.Max(maxEnergized, Simulate(map, f1));
            maxEnergized = Math.Max(maxEnergized, Simulate(map, f2));
        }

        for (var x = 0; x < map.Width; x++)
        {
            var f1 = new LightFront(new Position(x, 0), Direction.Down);
            var f2 = new LightFront(new Position(x, map.Height - 1), Direction.Up);
    
            maxEnergized = Math.Max(maxEnergized, Simulate(map, f1));
            maxEnergized = Math.Max(maxEnergized, Simulate(map, f2));
        }

        return maxEnergized;
    }

    public static int Simulate(Map map, LightFront start)
    {
        LightFront? nextFront = start;
        var remainingFronts = new Queue<LightFront>();

        var visitedPositions = new HashSet<Position>();
        var visitedFronts = new HashSet<LightFront>();

        var nextFrontsBuffer = new LightFront[2];

        while (nextFront != null || remainingFronts.Any())
        {
            var currentFront = nextFront ?? remainingFronts.Dequeue();
            nextFront = null;

            visitedPositions.Add(currentFront.Position);
            visitedFronts.Add(currentFront);

            var currentTile = map.Get(currentFront.Position);
            var numNextFronts = currentFront.Move(currentTile, nextFrontsBuffer);

            for (var i = 0; i < numNextFronts; i++)
            {
                var next = nextFrontsBuffer[i];
                if (!map.Contains(next.Position) || visitedFronts.Contains(next))
                    continue;

                if (i == 0)
                    nextFront = next;
                else
                    remainingFronts.Enqueue(next);
            }
        }

        return visitedPositions.Count;
    }
}