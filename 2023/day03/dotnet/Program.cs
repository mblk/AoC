namespace day03;

public static class Program
{
    private record Pos(int Row, int Column);
    
    public static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines(args[0]);
        int width = lines[0].Length;
        int height = lines.Length;

        var allValidNumbers = new List<int>();
        var allGearNumbers = new Dictionary<Pos, List<int>>();

        var readingNumber = false;
        var numberBuffer = 0;
        var symbolBuffer = new HashSet<(Pos, char)>();

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                char c = lines[row][column];
                var isDigit = Char.IsDigit(c);
                if (isDigit)
                {
                    int digit = c - '0';
                    readingNumber = true;
                    numberBuffer = numberBuffer * 10 + digit;
                    foreach (var s in GetAdjacentSymbols(lines, row, column))
                        symbolBuffer.Add(s);
                }

                // End of number or end of line?
                var flushBuffer = readingNumber && (!isDigit || column == width - 1);
                if (flushBuffer)
                {
                    var isValidNumber = symbolBuffer.Any();
                    if (isValidNumber)
                    {
                        allValidNumbers.Add(numberBuffer);

                        foreach (var (pos, _) in symbolBuffer
                                     .Where(p => IsGearSymbol(p.Item2)))
                        {
                            if (!allGearNumbers.TryGetValue(pos, out var numbers))
                                allGearNumbers.Add(pos, numbers = new List<int>());
                            numbers.Add(numberBuffer);
                        }
                    }

                    // Reset buffer
                    readingNumber = false;
                    numberBuffer = 0;
                    symbolBuffer.Clear();
                }
            }
        }

        var sumOfAllValidNumbers = allValidNumbers
            .Sum();
        var sumOfGearRatios = allGearNumbers
            .Values
            .Where(nums => nums.Count == 2)
            .Select(nums => nums[0] * nums[1])
            .Sum();
        
        Console.WriteLine($"Part1: {sumOfAllValidNumbers}");
        Console.WriteLine($"Part2: {sumOfGearRatios}");
    }

    private static (Pos, char)[] GetAdjacentSymbols(string[] lines, int row, int column)
    {
        var symbols = new List<(Pos, char)>();

        int width = lines[0].Length;
        int height = lines.Length;

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            int effectiveRow = row + rowOffset;
            if (effectiveRow < 0 || effectiveRow >= height) continue;

            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                int effectiveColumn = column + columnOffset;
                if (effectiveColumn < 0 || effectiveColumn >= width) continue;

                char c = lines[effectiveRow][effectiveColumn];
                if (!IsSymbol(c)) continue;
                
                var pos = new Pos(effectiveRow, effectiveColumn);
                symbols.Add((pos, c));
            }
        }

        return symbols.ToArray();
    }

    private static bool IsSymbol(char c)
    {
        if (Char.IsDigit(c)) return false;
        if (c == '.') return false;
        return true;
    }

    private static bool IsGearSymbol(char c)
        => c == '*';
}