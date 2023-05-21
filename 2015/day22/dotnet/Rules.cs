namespace day22;

internal record Rules(Effect[] Effects,
    IReadOnlyDictionary<PlayerAction, Spell> Spells);

internal record Effect(string Name,
    SpellAction[] EntryActions,
    SpellAction[] TickActions,
    SpellAction[] ExitActions);

internal record Spell(int ManaCost, SpellAction[] Actions);

internal abstract record SpellAction;
internal record PlayerHealthAction(int Delta) : SpellAction;
internal record PlayerManaAction(int Delta) : SpellAction;
internal record PlayerArmorAction(int Delta) : SpellAction;
internal record BossHealthAction(int Delta) : SpellAction;
internal record StartEffectAction(int Duration, string EffectName) : SpellAction;
