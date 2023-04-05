namespace day19;

internal class Fabricator
{
    private readonly Data _data;

    public Fabricator(Data data)
    {
        _data = data;
    }

    public int Fabricate()
    {
        const string startSequence = "e";
        string endSequence = _data.CalibrationSequence;

        string currentSequence = endSequence;
        int numberOfSteps = 0;
        
        while (true)
        {
            //Console.WriteLine($"Current: '{currentSequence}'");

            if (currentSequence == startSequence)
                return numberOfSteps;

            var didSomething = false;
            
            foreach (var replacement in _data.Replacements)
            {
                int index = currentSequence.IndexOf(replacement.Output, StringComparison.Ordinal);
                if (index == -1) continue;

                // Console.WriteLine($"applying {replacement.Input} -> {replacement.Output} (in reverse)");
                
                string prefix = currentSequence[..index];
                string postfix = currentSequence[(index + replacement.Output.Length)..];

                currentSequence = prefix + replacement.Input + postfix;
                didSomething = true;
                numberOfSteps++;
            }

            if (!didSomething)
                throw new Exception($"Stuck in endless loop");
        }
    }
}