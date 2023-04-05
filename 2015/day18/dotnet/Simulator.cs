namespace day18;

internal class Simulator
{
    private readonly Grid _grid;

    public Simulator(Grid grid)
    {
        _grid = grid;
    }

    public void Tick()
    {
        DetermineNextState();
        ApplyNextState();
    }
    
    private void DetermineNextState()
    {
        for (int y = 0; y < _grid.Height; y++)
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                int activeNeighbors = GetActiveNeighborCount(x, y);
                var light = _grid.Lights[x, y];
                
                if (light.State)
                {
                    light.NextState = activeNeighbors is 2 or 3;
                }
                else
                {
                    light.NextState = activeNeighbors == 3;
                }
            }
        }
    }

    private void ApplyNextState()
    {
        for (int y = 0; y < _grid.Height; y++)
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                var light = _grid.Lights[x, y];
                light.State = light.NextState;
            }
        }
    }

    private int GetActiveNeighborCount(int x, int y)
    {
        int count = 0;
        
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int effX = x + dx;
                int effY = y + dy;

                if ((effX != x || effY != y) &&
                    0 <= effX && effX < _grid.Width &&
                    0 <= effY && effY < _grid.Height &&
                    _grid.Lights[effX,effY].State)
                {
                    count++;
                }
            }
        }

        return count;
    }
}