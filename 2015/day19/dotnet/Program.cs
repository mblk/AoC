namespace day19;

public static class Program
{
    public static void Main()
    {
        const string inputFile = "../../../input.txt";

        Data data = Parser.Parse(inputFile);
        
        var calibrator = new Calibrator(data);
        Console.WriteLine($"Part1: {calibrator.Calibrate()}");

        var fabricator = new Fabricator(data);
        Console.WriteLine($"Part2: {fabricator.Fabricate()}");
    }
}
