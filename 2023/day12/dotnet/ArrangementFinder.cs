using System.Runtime.CompilerServices;

namespace day12;

public class ArrangementFinder
{
    private readonly SpringState[] _states;
    private readonly int[] _groups;
    private readonly Dictionary<int, long> _cache = new();

    public static long FindArrangements(SpringRow springRow)
    {
        return new ArrangementFinder(springRow.States, springRow.Groups)
            .Solve(0, 0, 0);
    }

    private ArrangementFinder(SpringState[] states, int[] groups)
    {
        _states = states;
        _groups = groups;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long Solve(int statePos, int groupPos, int matched)
    {
        var key = MakeKey(statePos, groupPos, matched);
        if (!_cache.TryGetValue(key, out var count))
            _cache.Add(key, count = SolveInner(statePos, groupPos, matched));
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int MakeKey(int statesPos, int groupPos, int matched)
    {
        return statesPos | (matched << 8) | (groupPos << 16);

        // 50% slower for some reason:
        //return HashCode.Combine(statesPos, groupPos, matched);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long SolveInner(int statePos, int groupPos, int matched)
    {
        // Reached end?
        if (statePos == _states.Length)
            // Matched all groups?
            return groupPos == _groups.Length && matched == 0 ||
                   groupPos == _groups.Length - 1 && matched == _groups[^1]
                ? 1
                : 0;

        var state = _states[statePos];
        var count = 0L;

        // Operational or unknown?
        if (state != SpringState.Damaged)
        {
            // No open matches from previous state?
            if (matched == 0)
                count = Solve(statePos + 1, groupPos, 0);
            // Got open matches and they fit the current group?
            else if (groupPos < _groups.Length && _groups[groupPos] == matched)
                count = Solve(statePos + 1, groupPos + 1, 0);
            // else: Got open matches but they don't fit the current group => count=0
        }

        // Damaged or unknown?
        if (state != SpringState.Operational)
        {
            // Add damaged spring to open matches and move on.
            count += Solve(statePos + 1, groupPos, matched + 1);
        }

        return count;
    }
}