namespace day05;

public static class Program
{
    public static void Main()
    {
        var jumplist1 = ParseJumpList("example");
        var jumplist2 = ParseJumpList("input");
        
        Console.WriteLine($"Part1 example: {ExecuteJumpList(jumplist1)}");
        Console.WriteLine($"Part1        : {ExecuteJumpList(jumplist2)}");
        Console.WriteLine($"Part2 example: {ExecuteJumpList(jumplist1, true)}");
        Console.WriteLine($"Part2        : {ExecuteJumpList(jumplist2, true)}");
    }

    private static int[] ParseJumpList(string fileName)
    {
        return File.ReadAllLines($"../../../../{fileName}")
            .Select(Int32.Parse)
            .ToArray();
    }

    private static int ExecuteJumpList(IEnumerable<int> jumplist, bool enhanced = false)
    {
        var jumps = jumplist.ToArray();
        var position = 0;
        var steps = 0;

        while (position >= 0 && position < jumps.Length)
        {
            var offset = jumps[position];

            if (enhanced && offset >= 3)
                jumps[position]--;
            else
                jumps[position]++;

            position += offset;
            steps++;
        }

        return steps;
    }
}