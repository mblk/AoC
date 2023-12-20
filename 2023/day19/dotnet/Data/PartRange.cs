using System.Diagnostics;

namespace day19.Data;

public readonly record struct PartRange(ValueRange X, ValueRange M, ValueRange A, ValueRange S)
{
    public ValueRange GetRange(PartValue value) => value switch
    {
        PartValue.X => X,
        PartValue.M => M,
        PartValue.A => A,
        PartValue.S => S,
        _ => throw new UnreachableException(),
    };

    public PartRange WithRange(PartValue value, ValueRange range) => value switch
    {
        PartValue.X => this with { X = range },
        PartValue.M => this with { M = range },
        PartValue.A => this with { A = range },
        PartValue.S => this with { S = range },
        _ => throw new UnreachableException()
    };

    public long GetProductOfRanges()
    {
        checked
        {
            return (long)X.Count * (long)M.Count * (long)A.Count * (long)S.Count;
        }
    }
}