
const int puzzleInput = 34_000_000;

// Part1
(int house1, int presents1) = FindHouseWithPresents(puzzleInput, CalculatePresents1);
Console.WriteLine($"Part1: House={house1} Presents={presents1}");

// Part2
(int house2, int presents2) = FindHouseWithPresents(puzzleInput, CalculatePresents2);
Console.WriteLine($"Part2: House={house2} Presents={presents2}");

static (int,int) FindHouseWithPresents(int minimumPresentCount, Func<int, int> presentLogic)
{
    for (int houseNumber = 1;; houseNumber++)
    {
        int presents = presentLogic(houseNumber);
        if (presents >= minimumPresentCount)
            return (houseNumber, presents);
    }
}

static int CalculatePresents1(int numHouse)
    => GetElves(numHouse)
        .Aggregate(0, (presents, elf) => presents + elf * 10);

static int CalculatePresents2(int numHouse)
    => GetElves(numHouse)
        .Where(elf => numHouse <= elf * 50)
        .Aggregate(0, (presents, elf) => presents + elf * 11);

static IEnumerable<int> GetElves(int numHouse)
    => GetDivisors(numHouse);

static IEnumerable<int> GetDivisors(int number)
{
    if (number <= 1)
        yield return 1;
    
    yield return 1;
    yield return number;
    
    for (int i=2; i<=Math.Sqrt(number); i++)
    {
        if (number % i != 0) continue;
        yield return i;

        int other = number / i;
        if (other != i)
            yield return other;
    }
}
