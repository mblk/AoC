namespace day22;

internal static class RulesProvider
{
    public static Rules GetRules()
    {
        var effects = new Effect[]
        {
            new Effect("Shield",
                new SpellAction[] { new PlayerArmorAction(7) },
                new SpellAction[] {},
                new SpellAction[] { new PlayerArmorAction(-7) }),
            
            new Effect("Poison",
                new SpellAction[] {},
                new SpellAction[] { new BossHealthAction(-3) },
                new SpellAction[] {}),
            
            new Effect("Recharge",
                new SpellAction[] {},
                new SpellAction[] { new PlayerManaAction(101) },
                new SpellAction[] {}
            )
        };
        
        var spells = new Dictionary<PlayerAction, Spell>
        {
            {
                PlayerAction.MagicMissile,
                new Spell(53, new SpellAction[]
                {
                    new BossHealthAction(-4),
                })
            },
            
            {
                PlayerAction.Drain,
                new Spell(73, new SpellAction[]
                {
                    new BossHealthAction(-2),
                    new PlayerHealthAction(2),
                })
            },
            
            {
                PlayerAction.Shield, 
                new Spell(113, new SpellAction[]
                {
                    new StartEffectAction(6, "Shield"),
                })
            },
            
            {
                PlayerAction.Poison, 
                new Spell(173, new SpellAction[]
                {
                    new StartEffectAction(6, "Poison"),
                })
            },
            
            {
                PlayerAction.Recharge, 
                new Spell(229, new SpellAction[]
                {
                    new StartEffectAction(5, "Recharge"),
                })
            },
        };

        return new Rules(effects, spells);
    }
}
