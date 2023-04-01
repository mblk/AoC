using System.Collections.ObjectModel;
using day07.Nodes;

namespace day07;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        ReadOnlyDictionary<string, Node> nodes = Parser.Parse(inputFile);

        var resolver1 = new Resolver(nodes);
        uint valueOfAForPart1 = resolver1.Resolve("a");
        Console.WriteLine($"Part1: {valueOfAForPart1}");
        
        var resolver2 = new Resolver(nodes);
        resolver2.Override("b", valueOfAForPart1);
        uint valueOfAForPart2 = resolver2.Resolve("a");
        Console.WriteLine($"Part2: {valueOfAForPart2}");
    }
}