namespace day15;

public readonly record struct LensInBox(string Label, int FocalLength);

public class Box
{
    public List<LensInBox> Lenses { get; } = new();
}

public class State
{
    public Box[] Boxes { get; } = Enumerable.Range(0, 256).Select(_ => new Box()).ToArray();

    public int CalculateFocusPower() => Boxes
        .Select((box, boxIndex) => box.Lenses
            .Select((lens, lensIndex) => (1 + boxIndex) * (lensIndex + 1) * lens.FocalLength)
            .Sum())
        .Sum();
    
    public void Print()
    {
        Console.WriteLine();
        Console.WriteLine("== state ==");
        for (var i = 0; i < Boxes.Length; i++)
        {
            var box = Boxes[i];
            if (!box.Lenses.Any())
                continue;
            Console.WriteLine($"Box {i}: {String.Join(" ", box.Lenses
                .Select(x => $"[{x.Label} {x.FocalLength}]"))}");
        }
    }
}