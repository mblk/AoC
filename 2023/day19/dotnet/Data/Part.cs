using System.Diagnostics;

namespace day19.Data;

public readonly record struct Part(int X, int M, int A, int S)
{
    public int GetValue(PartValue partValue) => partValue switch
    {
        PartValue.X => X,
        PartValue.M => M,
        PartValue.A => A,
        PartValue.S => S,
        _ => throw new UnreachableException(),
    };

    public int GetSumOfValues() => X + M + A + S;
}

public enum PartValue
{
    X,
    M,
    A,
    S,
}