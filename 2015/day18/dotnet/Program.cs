namespace day18;

public static class Program
{
    public static void Main(string[] args)
    {
        string inputFile = "../../../input.txt";
        int numSteps = 100;
        //bool cornerAlwaysOn = false; // part1
        bool cornerAlwaysOn = true; // part2

        if (args.Any())
        {
            inputFile = args[0];

            if (args.Length >= 2 && Int32.TryParse(args[1], out int argsNumSteps))
                numSteps = argsNumSteps;

            if (args.Length >= 3 && Boolean.TryParse(args[2], out bool argsCornerAlwaysOn))
                cornerAlwaysOn = argsCornerAlwaysOn;
        }
        
        Console.WriteLine($"Input:           {inputFile}");
        Console.WriteLine($"NumSteps:        {numSteps}");
        Console.WriteLine($"CornersAlwaysOn: {cornerAlwaysOn}");

        Grid grid = Parser.Parse(inputFile, cornerAlwaysOn);

        // Console.WriteLine($"Initial:");
        // grid.Print();
        
        var simulator = new Simulator(grid);
        for(int i=1; i<=numSteps; i++)
        {
            simulator.Tick();
            
            //Console.WriteLine($"After {i} steps:");
            //grid.Print();
        }
        
        grid.Print();
    }
}