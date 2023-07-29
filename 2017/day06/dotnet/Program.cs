namespace day06;

internal class MemoryState
{
    public IEnumerable<int> Blocks { get; }

    public MemoryState(IEnumerable<int> blocks) {
        Blocks = blocks.ToArray();
    }

    public override int GetHashCode() {
        int hc = 0;
        foreach (var b in Blocks)
            hc += b.GetHashCode();
        return hc;
    }

    public override bool Equals(object? other) {
        return other is MemoryState ms &&
            Enumerable.SequenceEqual(ms.Blocks, this.Blocks);
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($"Missing input file");
            return;
        }

        var filename = args[0];
        var blocks = ReadInput(filename);
        var (part1, part2) = FindReallocationInfiniteLoop(blocks);

        Console.WriteLine($"Part1: {part1}");
        Console.WriteLine($"Part2: {part2}");
    }

    private static (int, int) FindReallocationInfiniteLoop(int[] blocks)
    {
        var prev_blocks = new Dictionary<MemoryState, int>();
        var num_steps = 0;

        while (true)
        {
            if (prev_blocks.TryGetValue(new MemoryState(blocks), out var prev_num_steps)) {
                return (num_steps, num_steps - prev_num_steps);
            }

            prev_blocks.Add(new MemoryState(blocks), num_steps);
            ReallocateMemory(blocks);
            num_steps++;
        }
    }

    private static void DumpBlocks(int[] blocks)
    {
        Console.Write("Blocks: ");
        foreach (var b in blocks) Console.Write($"{b} ");
        Console.WriteLine("");
    }

    private static int GetIndexOfLargestBlock(int[] blocks)
    {
        int maxValue = blocks.Max();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] == maxValue) return i;
        }
        throw new Exception("Must not reach");
    }

    private static void ReallocateMemory(int[] blocks)
    {
        int idx = GetIndexOfLargestBlock(blocks);
        int mem = blocks[idx];
        blocks[idx] = 0;
        idx = (idx + 1) % blocks.Length;

        while (mem > 0)
        {
            blocks[idx]++;
            mem--;
            idx = (idx + 1) % blocks.Length;
        }
    }

    private static int[] ReadInput(string filename)
    {
        return File.ReadAllText(filename)
            .Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Int32.Parse(s))
            .ToArray();
    }
}
