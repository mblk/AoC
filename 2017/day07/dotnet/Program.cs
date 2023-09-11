namespace day07;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Missing input file");
            return;
        }

        var nodes = ParseInputFile(args[0]).ToArray();

        foreach (var root in nodes.Where(n => n.Parent is null))
            Console.WriteLine($"Root: {root.Name}");

        PropagateWeights(nodes);
    }

    private class Node
    {
        public string Name { get; }
        public int Weight { get; }

        public Node? Parent { get; set; }
        public HashSet<Node> Children { get; } = new();

        public int TotalWeight { get; set; }
        public int VisitCount { get; set; }

        public Node(string name, int weight)
            => (Name, Weight) = (name, weight);

        public override bool Equals(object? other)
            => other is Node otherNode && otherNode.Name == Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }

    private static IEnumerable<Node> ParseInputFile(string filename)
    {
        var linesData = File.ReadLines(filename)
            .Select(ParseLine)
            .ToArray();

        var nodes = linesData
            .Select(d => new Node(d.Name, d.Weight))
            .ToDictionary(x => x.Name, x => x);

        foreach (var lineData in linesData)
        {
            var parentNode = nodes[lineData.Name];
            foreach (var childName in lineData.Children)
            {
                var childNode = nodes[childName];
                childNode.Parent = parentNode;
                parentNode.Children.Add(childNode);
            }
        }

        return nodes.Values;
    }

    private record LineData(string Name, int Weight, string[] Children);

    private static LineData ParseLine(string line)
    {
        var name = line[..line.IndexOf(' ')];
        var weight = Int32.Parse(line[(line.IndexOf('(') + 1)..line.IndexOf(')')]);

        var arrow = line.IndexOf(" -> ", StringComparison.Ordinal);
        var children = arrow != -1
            ? line[(arrow + 4)..].Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray()
            : Array.Empty<string>();

        return new LineData(name, weight, children);
    }

    private static void PropagateWeights(IEnumerable<Node> nodes)
    {
        var leafs = nodes.Where(n => !n.Children.Any());
        var nodesToVisit = new Queue<Node>(leafs);

        while (nodesToVisit.Any())
        {
            var currentNode = nodesToVisit.Dequeue();
            currentNode.TotalWeight += currentNode.Weight;

            if (!ValidateNodeWeights(currentNode))
                return;

            var parentNode = currentNode.Parent;
            if (parentNode is null) continue;

            parentNode.TotalWeight += currentNode.TotalWeight;
            parentNode.VisitCount++;
            if (parentNode.VisitCount == parentNode.Children.Count)
                nodesToVisit.Enqueue(parentNode);
        }
    }

    private static bool ValidateNodeWeights(Node node)
    {
        // Assumption: At most one weight is incorrect.

        if (!node.Children.Any())
            return true;

        var totalChildWeights = node.Children
            .Select(c => c.TotalWeight)
            .GroupBy(x => x)
            .ToArray();

        if (totalChildWeights.Length == 1)
            return true;

        var badTotalWeight = totalChildWeights.Single(g => g.Count() == 1).Key;
        var goodTotalWeight = totalChildWeights.Single(g => g.Count() > 1).Key;
        var badChild = node.Children.Single(c => c.TotalWeight == badTotalWeight);
        var weightError = goodTotalWeight - badTotalWeight;
        var correctedOwnWeight = badChild.Weight + weightError;

        Console.WriteLine($"Weight of {badChild.Name} is {badChild.Weight} but should be {correctedOwnWeight}");
        return false;
    }
}