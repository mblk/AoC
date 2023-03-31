namespace day06;

internal enum Command
{
    TurnOn,
    TurnOff,
    Toggle
}

internal static class Parser
{
    private record CommandKeyword(Command Command, string Keyword);

    private static readonly CommandKeyword[] CommandKeywords = new[]
    {
        new CommandKeyword(Command.TurnOn, "turn on"),
        new CommandKeyword(Command.TurnOff, "turn off"),
        new CommandKeyword(Command.Toggle, "toggle")
    };
    
    public static (Command, Coordinate, Coordinate) ParseLine(string input, int fieldDimension)
    {
        // Find matching command.
        var matchingCommandKeyword = CommandKeywords
            .SingleOrDefault(x => input.StartsWith(x.Keyword))
            ?? throw new ArgumentException($"Invalid command in input '{input}'");

        string arguments = input[matchingCommandKeyword.Keyword.Length..].Trim();
        
        // Parse arguments (x,y through x,y).
        string[] splitArgs = arguments.Split(' ');
        if (splitArgs is not [_, "through", _])
            throw new ArgumentException($"Invalid arguments '{arguments}'");

        var startCoordinate = Coordinate.Parse(splitArgs[0], fieldDimension);
        var endCoordinate = Coordinate.Parse(splitArgs[2], fieldDimension);

        // Validate arguments
        if (startCoordinate.X > endCoordinate.X ||
            startCoordinate.Y > endCoordinate.Y)
            throw new Exception($"Invalid coordinate pair (start {startCoordinate}, end {endCoordinate})");

        return (matchingCommandKeyword.Command, startCoordinate, endCoordinate);
    }
}