namespace day22;

internal class BattleSimulator
{
    private readonly Rules _rules;
    private readonly State _startingState;
    private readonly bool _enableExtraPlayerDamage;

    private int _bestSolutionMana = Int32.MaxValue;
    private State _bestSolutionState;
    private PlayerAction[] _bestSolutionActions = Array.Empty<PlayerAction>();
    
    public BattleSimulator(Rules rules, State startingState, bool enableExtraPlayerDamage = false)
    {
        _rules = rules;
        _startingState = startingState;
        _enableExtraPlayerDamage = enableExtraPlayerDamage;
    }

    public void Simulate()
    {
        Console.WriteLine("Simulating battles ...");
        
        SimRecursive(_startingState, Array.Empty<PlayerAction>());

        if (_bestSolutionMana == Int32.MaxValue)
        {
            Console.WriteLine("No solution. The player never won.");
            return;
        }

        Console.WriteLine($"Mana spent:  {_bestSolutionMana}");
        Console.WriteLine($"Final state: {_bestSolutionState}");
        Console.WriteLine($"Actions:     {String.Join(",", _bestSolutionActions)}");
    }

    private void SimRecursive(State oldState, PlayerAction[] previousActions)
    {
        State state = oldState;

        // Player turn 1/3: optionally apply extra damage?
        if (_enableExtraPlayerDamage)
        {
            state.PlayerHealth -= 1;
            if (CheckIfBattleIsOverAndReportResult(state, previousActions))
                return;
        }

        // Player turn 2/3: Apply effects.
        state = ApplyActiveEffects(state);
        if (CheckIfBattleIsOverAndReportResult(state, previousActions))
            return;
        
        foreach (var action in GetLegalPlayerActions(state))
        {
            var nextPreviousActions = previousActions
                .AddItemToArray(action);
            
            var subState = state;

            // Player turn 3/3: Perform player action.
            subState = DoPlayerAction(subState, action);
            
            // Abort early?
            if (subState.TotalManaSpent >= _bestSolutionMana)
                return;
            
            if (CheckIfBattleIsOverAndReportResult(subState, nextPreviousActions))
                continue;
            
            // Boss turn 1/2: Apply effects.
            subState = ApplyActiveEffects(subState);
            if (CheckIfBattleIsOverAndReportResult(subState, nextPreviousActions))
                continue;
            
            // Boss turn 2/2: Perform boss action.
            subState = DoBossAction(subState);
            if (CheckIfBattleIsOverAndReportResult(subState, nextPreviousActions))
                continue;
            
            // Simulate next player turn in nested call. 
            SimRecursive(subState, nextPreviousActions);
        }
    }

    private bool CheckIfBattleIsOverAndReportResult(State state, PlayerAction[] actions)
    {
        if (BattleIsOver(state))
        {
            AddBattleResult(state, actions);
            return true;
        }
        return false;
    }
    
    private void AddBattleResult(State finalState, PlayerAction[] actions)
    {
        // Ignore boss wins.
        var playerWon = finalState.PlayerHealth > 0;
        if (!playerWon)
            return;

        // No improvement?
        if (finalState.TotalManaSpent >= _bestSolutionMana)
            return;
            
        // Store new best solution.
        _bestSolutionMana = finalState.TotalManaSpent;
        _bestSolutionState = finalState;
        _bestSolutionActions = actions;
    }

    private static bool BattleIsOver(State state)
    {
        var playerDead = state.PlayerHealth <= 0;
        var bossDead = state.BossHealth <= 0;

        if (playerDead && bossDead)
            throw new Exception("Internal error: Player and Boss dead!");

        return bossDead || playerDead;
    }
    
    private State ApplyActiveEffects(State state)
    {
        var newActiveEffects = new List<ActiveEffect>();
        
        foreach (var oldActiveEffect in state.ActiveEffects)
        {
            var activeEffect = oldActiveEffect; // local copy
            
            state = activeEffect.Effect.TickActions
                .Aggregate(state, ApplySpellAction);
            
            activeEffect.Timer--;
            if (activeEffect.Timer > 0)
                newActiveEffects.Add(activeEffect);
            else
                state = activeEffect.Effect.ExitActions
                    .Aggregate(state, ApplySpellAction);
        }

        state.ActiveEffects = newActiveEffects.ToArray();
        
        return state;
    }

    private State ApplySpellAction(State state, SpellAction spellAction)
    {
        switch (spellAction)
        {
            case PlayerHealthAction playerHealthAction:
                state.PlayerHealth += playerHealthAction.Delta;
                break;
                    
            case PlayerManaAction playerManaAction:
                state.PlayerMana += playerManaAction.Delta;
                break;
            
            case PlayerArmorAction playerArmorAction:
                state.PlayerArmor += playerArmorAction.Delta;
                break;
                    
            case BossHealthAction bossHealthAction:
                state.BossHealth += bossHealthAction.Delta;
                break;
            
            case StartEffectAction startEffectAction:
            {
                var effect = _rules.Effects
                    .Single(e => e.Name == startEffectAction.EffectName);

                state.ActiveEffects = state.ActiveEffects.AddItemToArray(
                    new ActiveEffect(effect, startEffectAction.Duration));

                state = effect.EntryActions
                    .Aggregate(state, ApplySpellAction);

                break;
            }
                    
            default:
                throw new NotImplementedException($"Spell action not implemented: {spellAction.GetType().FullName}");
        }
        
        return state;
    }

    private PlayerAction[] GetLegalPlayerActions(State state)
    {
        var result = new List<PlayerAction>();

        foreach (var (action, spell) in _rules.Spells)
        {
            // Not enough mana?
            if (state.PlayerMana < spell.ManaCost)
                continue;

            // Would this action start any effects which are still active?
            var effectNames = spell.Actions
                .OfType<StartEffectAction>()
                .Select(x => x.EffectName)
                .ToArray();
            if (state.ActiveEffects.Any(actEff => effectNames.Contains(actEff.Effect.Name)))
                continue;

            result.Add(action);
        }

        return result.ToArray();
    }

    private State DoPlayerAction(State state, PlayerAction playerAction)
    {
        var spell = _rules.Spells[playerAction];

        if (state.PlayerMana < spell.ManaCost)
            throw new Exception("Internal error: Not enough mana for spell");
        
        state.TotalManaSpent += spell.ManaCost;
        state.PlayerMana -= spell.ManaCost;

        state = spell.Actions.Aggregate(state, ApplySpellAction);

        return state;
    }
    
    private static State DoBossAction(State state)
    {
        int damage = Math.Max(1, state.BossDamage - state.PlayerArmor);
        
        state.PlayerHealth -= damage;

        return state;
    }
}