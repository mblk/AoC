namespace day12;

public enum SpringState
{
    Operational,
    Damaged,
    Unknown,
}

public record SpringRow(SpringState[] States, int[] Groups)
{
    public SpringRow Expand(int count)
    {
        var states = new List<SpringState>();
        var groups = new List<int>();

        for (var i = 0; i < count; i++)
        {
            states.AddRange(States);

            if (i < count - 1)
                states.Add(SpringState.Unknown);

            groups.AddRange(Groups);
        }

        return new SpringRow(states.ToArray(), groups.ToArray());
    }
}