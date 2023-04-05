namespace day19;

internal class Calibrator
{
    private readonly Data _data;

    public Calibrator(Data data)
    {
        _data = data;
    }

    public int Calibrate()
    {
        var seq = _data.CalibrationSequence;

        var allPossibleModifications = new HashSet<string>();

        foreach (var replacement in _data.Replacements)
        {
            int currentPosition = 0;

            while (true)
            {
                int index = seq.IndexOf(replacement.Input, currentPosition, StringComparison.Ordinal);
                if (index == -1) break;
                currentPosition += replacement.Input.Length;

                string prefix = seq[..index];
                string postfix = seq[(index + replacement.Input.Length)..];

                string modifiedSequence = prefix + replacement.Output + postfix;

                allPossibleModifications.Add(modifiedSequence);
            }
        }

        return allPossibleModifications.Count;
    }
}