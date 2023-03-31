namespace day06;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";
        const int fieldDimension = 1000;
       
        ProcessLightInstructions(inputFile, fieldDimension, Part1_LightLogic, Part1_EvaluationLogic);
        ProcessLightInstructions(inputFile, fieldDimension, Part2_LightLogic, Part2_EvaluationLogic);
    }

    private delegate int LightLogicHandler(Command command, int oldValue);

    private delegate void EvaluationLogicHandler(int[,] lights);
    
    private static void ProcessLightInstructions(string filename, int fieldDimension,
        LightLogicHandler lightLogic, EvaluationLogicHandler evaluationLogic)
    {
        var lights = new int[fieldDimension,fieldDimension];
        
        foreach (string line in File.ReadLines(filename))
        {
            var (command, start, stop) = Parser.ParseLine(line, fieldDimension);
            
            for (int x = start.X; x <= stop.X; x++)
            for (int y = start.Y; y <= stop.Y; y++)
                lights[x, y] = lightLogic(command, lights[x, y]);
        }

        evaluationLogic(lights);
    }

    private static int Part1_LightLogic(Command command, int oldValue)
        => command switch
        {
            Command.TurnOn => 1,
            Command.TurnOff => 0,
            Command.Toggle => (oldValue + 1) % 2,
            _ => oldValue
        };
    
    private static void Part1_EvaluationLogic(int[,] lights)
    {
        var numLit = 0;
        for(var x=0; x<lights.GetLength(0); x++)
        for(var y=0; y<lights.GetLength(1); y++)
            if (lights[x, y] > 0)
                numLit++;
        
        Console.WriteLine($"Part1 - num lit: {numLit}");
        PngWriter.WriteBlackAndWhite(lights, "part1.png");
    }
    
    private static int Part2_LightLogic(Command command, int oldValue)
        => command switch
        {
            Command.TurnOn => oldValue + 1,
            Command.TurnOff => Math.Max(0, oldValue - 1),
            Command.Toggle => oldValue + 2,
            _ => oldValue
        };
    
    private static void Part2_EvaluationLogic(int[,] lights)
    {
        var totalBrightness = 0;
        for(var x=0; x<lights.GetLength(0); x++)
        for(var y=0; y<lights.GetLength(1); y++)
            totalBrightness += lights[x, y];
        
        Console.WriteLine($"Part2 - total brightness: {totalBrightness}");
        PngWriter.WriteGrayscale(lights, "part2.png");
    }
}
