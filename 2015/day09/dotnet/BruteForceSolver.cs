namespace day09;

internal class BruteForceSolver
{
    private readonly Data _data;

    public BruteForceSolver(Data data)
    {
        _data = data;
    }

    public (int,int) Solve()
    {
        int numNodes = _data.Nodes.Length;
        var indices = new int[numNodes];

        int lowestCost = Int32.MaxValue;
        int[] shortestRoute = Array.Empty<int>();

        int highestCost = Int32.MinValue;
        int[] longestRoute = Array.Empty<int>();

        bool increaseIndex(int num)
        {
            if (num >= numNodes)
                return false;
            indices[num]++;
            if (indices[num] >= numNodes)
            {
                indices[num] = 0;
                return increaseIndex(num + 1);
            }
            return true;
        }
        
        while (true)
        {
            if (!increaseIndex(0))
                break;

            if (!IsValidIndex(indices))
                continue;

            int cost = CalculateCost(indices);

            if (cost < lowestCost)
            {
                lowestCost = cost;
                shortestRoute = indices.ToArray();
            }

            if (cost > highestCost)
            {
                highestCost = cost;
                longestRoute = indices.ToArray();
            }
        }

        return (lowestCost, highestCost);
    }

    private bool IsValidIndex(int[] indices)
    {
        if (indices.Length != indices.Distinct().Count())
            return false;

        return true;
    }

    private int CalculateCost(int[] indices)
    {
        int totalCost = 0;
        
        for (int i = 0; i < indices.Length - 1; i++)
        {
            int thisIndex = indices[i];
            int nextIndex = indices[i + 1];

            string thisNode = _data.Nodes[thisIndex];
            string nextNode = _data.Nodes[nextIndex];

            var edge = _data.Edges.SingleOrDefault(x => x.Item1 == thisNode && x.Item2 == nextNode);
            if (edge is (null, null, 0))
                return -1;

            totalCost += edge.Item3;
        }

        return totalCost;
    }
}