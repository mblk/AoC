namespace day14;

internal static class Parser
{
    public static ReindeerStats[] Parse(string filename)
    {
        var result = new List<ReindeerStats>();

        foreach (string line in File.ReadLines(filename))
        {
            string[] parts = line.TrimEnd('.').Split(' ');

            if (parts is not
                [
                    var name, "can", "fly", var speed, "km/s", "for",
                    var flyTime, "seconds,", "but", "then", "must",
                    "rest", "for", var restTime, "seconds"
                ])
                throw new Exception($"Invalid input");
            
            if (!Int32.TryParse(speed, out int speedValue) ||
                !Int32.TryParse(flyTime, out int flyTimeValue) ||
                !Int32.TryParse(restTime, out int restTimeValue))
                throw new Exception($"Invalid input");

            result.Add(new ReindeerStats(name, speedValue, flyTimeValue, restTimeValue));
        }

        return result.ToArray();
    }
}