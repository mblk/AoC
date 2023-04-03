namespace day14;

internal record SimulationResult(int MaxDistanceAtEnd, int MaxPoints);

internal record Simulator
{
    private readonly ReindeerStats[] _stats;

    public Simulator(IEnumerable<ReindeerStats> stats)
    {
        _stats = stats.ToArray();
    }

    public SimulationResult Simulate(int simulationTime)
    {
        var reindeers = _stats
            .Select(stat => new Reindeer(stat))
            .ToArray();

        for (int t = 0; t < simulationTime; t++)
        {
            foreach (var reindeer in reindeers)
                reindeer.Tick();

            int maxDistance = reindeers.Max(r => r.Distance);
            
            foreach (var reindeer in reindeers.Where(r => r.Distance == maxDistance))
                reindeer.AwardPoint();
        }

        int maxDistanceAtEnd = reindeers.Max(r => r.Distance);
        int maxPointsAtEnd = reindeers.Max(r => r.Points);

        return new SimulationResult(maxDistanceAtEnd, maxPointsAtEnd);
    }
}
