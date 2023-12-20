namespace day19.Data;

public readonly struct ValueRange
{
    public readonly int Min;
    public readonly int Max;
    public readonly int Count;
    
    public ValueRange(int min, int max)
    {
        if (min >= max) throw new ArgumentException("min >= max");
        
        Min = min;
        Max = max;
        Count = max - min + 1;
    }
}