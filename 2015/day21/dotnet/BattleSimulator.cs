namespace day21;

internal class BattleSimulator
{
    private readonly Player[] _players;
    private int _currentPlayerIndex;
    
    public BattleSimulator(IEnumerable<Player> players)
    {
        _players = players.ToArray();
        _currentPlayerIndex = 0;
    }

    public Player Simulate()
    {
        while (true)
        {
            int nextPlayerIndex = (_currentPlayerIndex + 1) % _players.Length;

            var currentPlayer = _players[_currentPlayerIndex];
            var nextPlayer = _players[nextPlayerIndex];

            var damageToDeal = Math.Max(1, currentPlayer.Damage - nextPlayer.Armor);
            
            nextPlayer.CurrentHitpoints -= damageToDeal;
            
            //Console.WriteLine($"{currentPlayer.Name} deals {damageToDeal} damage to {nextPlayer.Name}, HP reduced to {nextPlayer.CurrentHitpoints}");

            if (nextPlayer.CurrentHitpoints <= 0)
            {
                // Console.WriteLine($"Player '{currentPlayer.Name}' wins");
                return currentPlayer;
            }
            
            _currentPlayerIndex = nextPlayerIndex;
        }
    }
}