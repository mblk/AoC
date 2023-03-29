
var input = File.ReadAllText(@"../../../input.txt");

Console.WriteLine($"Input length: {input.Length}");

// Part 1
{
    var position = new Position();
    var world = new World();

    world.Deliver(position);
    
    foreach (var c in input)
    {
        position.Move(c);
        world.Deliver(position);
    }

    Console.WriteLine($"Part1 - Houses which get one present: {world.Count}");
}

// Part 1b
{
    var santas = new Santas(1);
    var world = new World();

    santas.DeliverAndSelectNext(world);
    
    foreach (var c in input)
    {
        santas.Move(c);
        santas.DeliverAndSelectNext(world);
    }
    
    Console.WriteLine($"Part1b - Houses which get one present: {world.Count}");
}

// Part 2
{
    var santas = new Santas(2);
    var world = new World();

    santas.DeliverAndSelectNext(world);
    santas.DeliverAndSelectNext(world);
    
    foreach (var c in input)
    {
        santas.Move(c);
        santas.DeliverAndSelectNext(world);
    }
    
    Console.WriteLine($"Part2 - Houses which get one present: {world.Count}");
}

internal class World : Dictionary<Position, int>
{
    public void Deliver(Position position)
    {
        if (!base.TryGetValue(position, out var count))
            count = 0;
        base[position] = count + 1;
    }
}

internal class Santas
{
    private readonly Position[] _positions;
    private int _activeSanta;
    
    public Santas(int count)
    {
        _positions = Enumerable.Repeat(new Position(), count).ToArray();
    }

    public void DeliverAndSelectNext(World world)
    {
        world.Deliver(_positions[_activeSanta]);
        _activeSanta = (_activeSanta + 1) % _positions.Length;
    }

    public void Move(char direction)
    {
        _positions[_activeSanta].Move(direction);
    }
}

internal struct Position
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Position() { }

    public void Move(char direction)
    {
        switch (direction)
        {
            case '^': Y++; break;
            case 'v': Y--; break;
            case '>': X++; break;
            case '<': X--; break;
            default: throw new ArgumentException($"Invalid direction {direction}");
        }
    }

    public override bool Equals(object? obj)
        => obj is Position other && X == other.X && Y == other.Y;

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public override string ToString()
        => $"({X},{Y})";
}