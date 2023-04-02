namespace day09;

internal static class Parser
{
    public static Data Parse(string filename)
    {
        var nodes = new HashSet<string>();
        var edges = new List<(string, string, int)>();

        foreach (string line in File.ReadLines(filename))
        {
            string[] parts = line.Split(' ');

            if (parts is not [var a, "to", var b, "=", var c] ||
                !Int32.TryParse(c, out int cost))
                throw new Exception($"Invalid input");

            nodes.Add(a);
            nodes.Add(b);
            edges.Add((a, b, cost));
            edges.Add((b, a, cost));
        }

        return new Data(nodes, edges);
    }
}