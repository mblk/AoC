namespace day22;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello!");

        var startingState = new State()
        {
            // // Example 1
            // PlayerHealth = 10,
            // PlayerMana = 250,
            // BossHealth = 13,
            // BossDamage = 8,
            
            // Example 2
            // PlayerHealth = 10,
            // PlayerMana = 250,
            // BossHealth = 14,
            // BossDamage = 8,
            
            // Actual puzzle
            PlayerHealth = 50,
            PlayerMana = 500,
            BossHealth = 51,
            BossDamage = 9,
        };

        var rules = RulesProvider.GetRules();

        Console.WriteLine("=== Part 1 ===");
        new BattleSimulator(rules, startingState)
            .Simulate();
        
        Console.WriteLine("=== Part 2 ===");
        new BattleSimulator(rules, startingState, enableExtraPlayerDamage: true)
            .Simulate();
    }
}