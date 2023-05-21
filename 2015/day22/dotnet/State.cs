using System.Text;

namespace day22;

internal struct ActiveEffect
{
    public ActiveEffect(Effect effect, int timer)
    {
        Effect = effect;
        Timer = timer;
    }
    
    public readonly Effect Effect;
    public int Timer;
}

internal struct State
{
    public State()
    {
    }

    public int PlayerHealth;
    public int PlayerMana;
    public int PlayerArmor;

    public int BossHealth;
    public int BossDamage;

    public ActiveEffect[] ActiveEffects = Array.Empty<ActiveEffect>();
    
    public int TotalManaSpent;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"Player {PlayerHealth} HP, {PlayerMana} Mana, {PlayerArmor} Armor");
        sb.Append($" | Boss {BossHealth} HP {BossDamage} DMG");

        foreach (var activeEffect in ActiveEffects)
            sb.Append($" | {activeEffect.Effect.Name}({activeEffect.Timer})");
        
        sb.Append($" | TotalSpent={TotalManaSpent}");

        return sb.ToString();
    }
}