namespace day02;

public static class Program
{
    private record Game(int Id, Draw[] Draws);
    private record Draw(int Red, int Green, int Blue);

    public static void Main(string[] args)
    {
        var games = File.ReadAllLines(args[0])
            .Select(ParseGame)
            .ToArray();
        
        var part1 = games
            .Where(IsPossibleGame)
            .Sum(g => g.Id);

        var part2 = games
            .Select(GetPowerOfMinimumSetOfCubes)
            .Sum();

        Console.WriteLine($"Part1: {part1}");
        Console.WriteLine($"Part2: {part2}");
    }

    private static Draw GetMinimumNumberOfCubes(Game game)
    {
        var minRed = game.Draws.Max(d => d.Red);
        var minGreen = game.Draws.Max(d => d.Green);
        var minBlue = game.Draws.Max(d => d.Blue);
        return new Draw(minRed, minGreen, minBlue);
    }

    private static int GetPowerOfMinimumSetOfCubes(Game game)
    {
        var minSet = GetMinimumNumberOfCubes(game);
        return minSet.Red * minSet.Green * minSet.Blue;
    }

    private static bool IsPossibleGame(Game game)
    {
        const int maxRed = 12;
        const int maxGreen = 13;
        const int maxBlue = 14;

        bool failCondition(Draw draw)
        {
            return draw.Red > maxRed ||
                   draw.Green > maxGreen ||
                   draw.Blue > maxBlue;
        }

        return !game.Draws.Any(failCondition);
    }

    private static Game ParseGame(string input)
    {
        var idxColon = input.IndexOf(':');
        var id = Int32.Parse(input[5..idxColon]);

        var draws = input[(idxColon + 1)..]
            .Trim()
            .Split(';')
            .Select(ParseDraw)
            .ToArray();

        return new Game(id, draws);
    }

    private static Draw ParseDraw(string input)
    {
        var red = 0;
        var green = 0;
        var blue = 0;

        foreach (var item in input.Trim().Split(','))
        {
            var parts = item.Trim().Split(' ');
            if (parts.Length != 2) throw new Exception($"Invalid draw item: {item}");
            var count = Int32.Parse(parts[0]);
            var color = parts[1];
            switch (color)
            {
                case "red": red += count; break;
                case "green": green += count; break;
                case "blue": blue += count; break;
                default: throw new Exception($"invalid color: {color}");
            }
        }

        return new Draw(red, green, blue);
    }
}