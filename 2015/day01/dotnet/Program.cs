
var input = File.ReadAllText(@"../../../input.txt");

var floor = 0;
int? firstPositionEnteringBasement = null;

for (var i = 0; i < input.Length; i++)
{
    switch (input[i])
    {
        case '(':
            floor++;
            break;
        
        case ')':
            floor--;
            if (!firstPositionEnteringBasement.HasValue && floor < 0)
                firstPositionEnteringBasement = i + 1;
            break;
    }
}

Console.WriteLine($"Floor: {floor}");
Console.WriteLine($"Entered basement for the first time at position: {firstPositionEnteringBasement}");

var isCorrect = floor == 138 && firstPositionEnteringBasement == 1771;
Console.WriteLine($"IsCorrect: {isCorrect}");
