namespace day15;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        Ingredient[] ingredients = Parser.Parse(inputFile);

        var solver = new Solver(ingredients, 100);
        
        Console.WriteLine($"Part1: {solver.Solve(null)}");
        Console.WriteLine($"Part2: {solver.Solve(500)}");
    }
}
