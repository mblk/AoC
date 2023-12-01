var spelledOutDigits = new Dictionary<string, int>
{
    { "one", 1 },
    { "two", 2 },
    { "three", 3 },
    { "four", 4 },
    { "five", 5 },
    { "six", 6 },
    { "seven", 7 },
    { "eight", 8 },
    { "nine", 9 },
};

var lines = File.ReadAllLines(args[0]);
var part1 = lines.Select(ParseCalibrationValue1).Sum();
var part2 = lines.Select(ParseCalibrationValue2).Sum();
Console.WriteLine($"Part1: {part1}");
Console.WriteLine($"Part2: {part2}");
return;

int ParseCalibrationValue1(string line)
{
    int[] digits = line.ToCharArray()
        .Where(Char.IsNumber)
        .Select(c => c - '0')
        .ToArray();
    
    return CreateCalibrationValue(digits);
}

int ParseCalibrationValue2(string line)
{
    var digits = new List<int>();

    for (int i = 0; i < line.Length; i++)
    {
        if (Char.IsNumber(line[i]))
        {
            digits.Add(line[i] - '0');
        }
        else if (IsSpelledOutDigit(line.AsSpan(i), out int value))
        {
            digits.Add(value);
        }
    }

    return CreateCalibrationValue(digits);
}

int CreateCalibrationValue(IReadOnlyList<int> digits)
{
    return digits.FirstOrDefault() * 10 + digits.LastOrDefault();
}

bool IsSpelledOutDigit(ReadOnlySpan<char> input, out int value)
{
    foreach (var (s, v) in spelledOutDigits)
    {
        if (input.StartsWith(s))
        {
            value = v;
            return true;
        }
    }

    value = 0;
    return false;
}