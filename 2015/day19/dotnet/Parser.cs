namespace day19;

internal record Replacement(string Input, string Output);

internal record Data(Replacement[] Replacements, string CalibrationSequence);

internal static class Parser
{
    public static Data Parse(string inputFile)
    {
        var replacements = new List<Replacement>();
        var calibrationSequence = String.Empty;
        
        foreach (string line in File.ReadLines(inputFile))
        {
            if (String.IsNullOrWhiteSpace(line)) continue;
            
            if (line.Contains(" => "))
            {
                string[] parts = line.Split(" => ");
                if (parts.Length != 2) throw new Exception("invalid input");
                
                replacements.Add(new Replacement(parts[0], parts[1]));
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(calibrationSequence))
                    throw new Exception("invalid input");
                calibrationSequence = line;
            }
        }

        return new Data(replacements.ToArray(), calibrationSequence);
    }
}