namespace day21;

public static class Program
{
    public static void Main()
    {
        var itemDatabase = new ItemDatabase();
        var playerFactory = new PlayerFactory(itemDatabase);

        var playerConfig = PlayerConfig.GetFirst();

        var lowestWinningCost = Int32.MaxValue;
        var highestLosingCost = Int32.MinValue;
        
        PlayerConfig? cheapestWinningPlayerConfig = null;
        PlayerConfig? mostExpensiveLosingPlayerConfig = null;
        
        while (true)
        {
            var boss = playerFactory.GetBoss();
            (Player player, int cost) = playerFactory.GetPlayerAndCost(playerConfig);
            
            var simulator = new BattleSimulator(new[] { player, boss });
            var winner = simulator.Simulate();

            if (winner == player)
            {
                if (cost < lowestWinningCost)
                {
                    lowestWinningCost = cost;
                    cheapestWinningPlayerConfig = playerConfig;
                }
            }
            else
            {
                if (cost > highestLosingCost)
                {
                    highestLosingCost = cost;
                    mostExpensiveLosingPlayerConfig = playerConfig;
                }
            }
            
            playerConfig = playerConfig.Next(itemDatabase);
            if (playerConfig is null) break;
        }

        Console.WriteLine($"Part1:");
        Console.WriteLine($"Lowest winning cost: {lowestWinningCost}");
        Console.WriteLine($"{cheapestWinningPlayerConfig}");

        Console.WriteLine($"Part2:");
        Console.WriteLine($"Highest losing cost: {highestLosingCost}");
        Console.WriteLine($"{mostExpensiveLosingPlayerConfig}");
    }
}