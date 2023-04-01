
const string inputFile = @"../../../input.txt";

// part1
{
    var totalCharactersOfCode = 0;
    var totalCharactersInMemory = 0;

    foreach (string rawLine in File.ReadLines(inputFile))
    {
        var charactersOfCode = 0;
        var charactersInMemory = 0;

        string trimmedLine = rawLine.Trim();
        if (!trimmedLine.StartsWith("\"") || !trimmedLine.EndsWith("\"")) throw new Exception("Invalid line");
        charactersOfCode += trimmedLine.Length;

        string inner = trimmedLine[1..^1];

        for (int i = 0; i < inner.Length; i++, charactersInMemory++)
        {
            if (inner[i] != '\\') continue;
            
            int extraChars = inner.Length - i - 1;
            
            // \\ or \" ?
            if (extraChars >= 1 && (inner[i+1] == '\\' || inner[i+1] == '\"'))
            {
                i += 1;
            }
            // \x00 ?
            else if (extraChars >= 3 && inner[i+1] == 'x' &&
                     Char.IsAsciiHexDigit(inner[i+2]) &&
                     Char.IsAsciiHexDigit(inner[i+3]))
            {
                i += 3;
            }
        }

        totalCharactersOfCode += charactersOfCode;
        totalCharactersInMemory += charactersInMemory;
    }

    Console.WriteLine($"Part1: {totalCharactersOfCode - totalCharactersInMemory}");
}

// part2
{
    var totalCharactersBefore = 0;
    var totalCharactersAfter = 0;
    
    foreach (string line in File.ReadLines(inputFile))
    {
        var charactersBefore = 0;
        var charactersAfter = 0;

        charactersBefore = line.Length;
        charactersAfter += 2; // start/end quote

        for (int i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case '\"':
                case '\\':
                    charactersAfter += 2;
                    break;
                
                default:
                    charactersAfter++;
                    break;
            }
        }

        totalCharactersBefore += charactersBefore;
        totalCharactersAfter += charactersAfter;
    }
    
    Console.WriteLine($"Part2: {totalCharactersAfter - totalCharactersBefore}");
}