namespace day18;

internal class Light
{
    private readonly bool _stuckAlwaysOn;

    private bool _state;

    public bool State
    {
        get => _state || _stuckAlwaysOn;
        set => _state = value;
    }
    
    public bool NextState { get; set; }

    public Light(bool stuckAlwaysOn = false)
    {
        _stuckAlwaysOn = stuckAlwaysOn;
    }
}