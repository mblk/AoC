
const string originalPassword = "vzbxkghb";

var newPassword1 = GetNextValidPassword(originalPassword);
var newPassword2 = GetNextValidPassword(newPassword1);

Console.WriteLine($"Part1: {newPassword1}");
Console.WriteLine($"Part2: {newPassword2}");

string GetNextValidPassword(string oldPassword)
{
    var passwordCandidate = oldPassword;
    while (true)
    {
        passwordCandidate = IncrementString(passwordCandidate);
        if (IsValidPassword(passwordCandidate))
            return passwordCandidate;
    }
}

bool IsValidPassword(string input)
{
    var characters = input.ToCharArray();
    
    bool hasIncreasingStraight = false;
    bool hasIllegalCharacters = false;
    var pairs = new HashSet<char>();
    
    for (int i = 0; i < input.Length; i++)
    {
        int remaining = input.Length - i - 1;
        
        if (remaining >= 2 &&
            characters[i] <= 'x' &&
            characters[i+1] == characters[i]+1 &&
            characters[i+2] == characters[i]+2)
        {
            hasIncreasingStraight = true;
        }

        switch (characters[i])
        {
            case 'i':
            case 'o':
            case 'l':
                hasIllegalCharacters = true;
                break;
        }

        if (remaining >= 1 && characters[i] == characters[i+1])
        {
            pairs.Add(characters[i]);
        }
    }

    var hasTwoDifferentPairs = pairs.Count >= 2;

    return hasIncreasingStraight && !hasIllegalCharacters && hasTwoDifferentPairs;
}

string IncrementString(string input)
{
    char[] characters = input.ToCharArray();

    var overflow = false;
    
    for (int i = input.Length - 1; i >= 0; i--)
    {
        characters[i]++;
        
        // break loop if no overflow occured at this position.
        if (characters[i] <= 'z')
            break;

        // overflow -> reset and continue with next position.
        characters[i] = 'a';

        // overflow at leftmost position?
        if (i == 0)
            overflow = true;
    }

    var result = new string(characters);
   
    if (overflow)
        result = 'a' + result;

    return result;
}

