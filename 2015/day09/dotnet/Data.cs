namespace day09;

internal class Data
{
    public string[] Nodes { get; }
    
    public (string,string,int)[] Edges { get; }

    public Data(IEnumerable<string> nodes, IEnumerable<(string, string, int)> edges)
    {
        Nodes = nodes.OrderBy(n => n).ToArray();
        Edges = edges.OrderBy(x => x.Item3).ToArray();
    }
}