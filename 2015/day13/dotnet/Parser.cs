namespace day13;

internal static class Parser
{
    public static SeatingInfo[] Parse(string filename)
    {
        var result = new List<SeatingInfo>();

        foreach (string line in File.ReadLines(filename))
        {
            string[] parts = line.TrimEnd('.').Split(' ');

            if (parts is not [var person, "would", var op, var value,
                    "happiness", "units", "by", "sitting", "next", "to", var neighbour])
                throw new ArgumentException($"Invalid input");
            
            if (!Int32.TryParse(value, out int intValue))
                throw new ArgumentException($"Invalid input");

            int change = op switch
            {
                "gain" => intValue,
                "lose" => -intValue,
                _ => throw new ArgumentException("Invalid object")
            };

            result.Add(new SeatingInfo(person, neighbour, change));
        }

        return result.ToArray();
    }
}