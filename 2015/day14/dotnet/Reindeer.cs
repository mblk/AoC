namespace day14;

internal class Reindeer
{
    private readonly ReindeerStats _stats;
    
    public bool IsFlying { get; private set; }
    public int TimeInState { get; private set; }
    public int Distance { get; private set; }
    public int Points { get; private set; }

    public Reindeer(ReindeerStats stats)
    {
        _stats = stats;

        Reset();
    }

    public void AwardPoint()
    {
        Points++;
    }

    private void Reset()
    {
        IsFlying = true;
        TimeInState = 0;
        Distance = 0;
        Points = 0;
    }

    public void Tick()
    {
        if (IsFlying)
        {
            Distance += _stats.Speed;
            TimeInState++;
        
            if (TimeInState >= _stats.FlyTime)
            {
                IsFlying = false;
                TimeInState = 0;
            }
        }
        else
        {
            TimeInState++;
        
            if (TimeInState >= _stats.RestTime)
            {
                IsFlying = true;
                TimeInState = 0;
            }
        }
    }
}
