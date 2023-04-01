using System.Collections.ObjectModel;
using day07.Nodes;

namespace day07;

internal static class Parser
{
    public static ReadOnlyDictionary<string, Node> Parse(string filename)
    {
        var nodes = new List<Node>();
        
        foreach (string line in File.ReadLines(filename))
        {
            string[] all = line.Split(' ');
            if (all is not [.., "->", var outputName])
                throw new Exception("Invalid input: '{line}");

            string[] input = all[..^2];
            switch (input)
            {
                case ["NOT", var a]:
                    nodes.Add(new NotNode(outputName, a));
                    break;

                case [var a, "AND", var b]:
                    nodes.Add(new AndNode(outputName, a, b));
                    break;

                case [var a, "OR", var b]:
                    nodes.Add(new OrNode(outputName, a, b));
                    break;

                case [var a, "RSHIFT", var b]:
                    nodes.Add(new RightShiftNode(outputName, a, b));
                    break;
                
                case [var a, "LSHIFT", var b]:
                    nodes.Add(new LeftShiftNode(outputName, a, b));
                    break;

                case [var a]:
                    nodes.Add(new WireNode(outputName, a));
                    break;
                
                default:
                    throw new Exception($"Invalid input: '{line}");
            }
        }

        return new ReadOnlyDictionary<string, Node>(nodes
            .ToDictionary(n => n.OutputName, n => n));
    }
}