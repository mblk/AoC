namespace day15;

internal record Effect(string Name, int Value);
internal record Ingredient(string Name, Effect[] Effects);

internal static class Parser
{
    public static Ingredient[] Parse(string filename)
    {
        var ingredients = new List<Ingredient>();
        
        foreach (string line in File.ReadLines(filename))
        {
            int colonIndex = line.IndexOf(':');
            if (colonIndex == -1) throw new Exception("invalid input");
            
            string ingredientName = line[..colonIndex].Trim();

            var effects = line[(colonIndex + 1)..]
                .Trim()
                .Split(',')
                .Select(ParseEffect)
                .ToArray();

            ingredients.Add(new Ingredient(ingredientName, effects));
        }

        return ingredients.ToArray();
    }

    private static Effect ParseEffect(string input)
    {
        string[] effectParts = input.Trim().Split(' ');
        if (effectParts.Length != 2)
            throw new Exception("Invalid input");
                
        string effectName = effectParts[0];

        if (!Int32.TryParse(effectParts[1], out int effectValue))
            throw new Exception("Invalid input");

        return new Effect(effectName, effectValue);
    }
}