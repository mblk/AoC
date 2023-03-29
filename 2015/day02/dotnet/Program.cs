
var totalArea = 0;
var totalRibbon = 0;

(int, int)[] permutations = { (0, 1), (1, 2), (0, 2) };

foreach (var line in File.ReadLines(@"../../../input.txt"))
{
    var numbers = line
        .Split('x')
        .Select(Int32.Parse)
        .ToArray();

    var sideAreas = permutations
        .Select(p => numbers[p.Item1] * numbers[p.Item2])
        .ToArray();
    
    var perimeterLengths = permutations
        .Select(p => 2 * (numbers[p.Item1] + numbers[p.Item2]))
        .ToArray();

    var volume = numbers.Aggregate(1, (acc, val) => acc * val);
    
    var area = 2 * sideAreas.Sum() + sideAreas.Min();
    var ribbon = perimeterLengths.Min() + volume;
    
    totalArea += area;
    totalRibbon += ribbon;
}

Console.WriteLine($"Total area: {totalArea}");
Console.WriteLine($"Total ribbon: {totalRibbon}");

var isCorrect = totalArea == 1586300 && totalRibbon == 3737498;
Console.WriteLine($"IsCorrect: {isCorrect}");
