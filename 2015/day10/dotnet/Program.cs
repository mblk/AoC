using System.Text;

const string input = "1113122113";

var currentSequence = input;
for (int i = 1; i <= 50; i++)
{
    currentSequence = LookAndSay(currentSequence);
    Console.WriteLine($"Length after {i} evolutions: {currentSequence.Length}");
}

string LookAndSay(string inputSequence)
{
    var result = new StringBuilder(inputSequence.Length * 2);

    var remainingSequence = inputSequence.AsSpan();
    
    while(remainingSequence.Length > 0)
    {
        (char c, int count) = GetRun(remainingSequence);

        // move start forward by 'count' characters.
        remainingSequence = remainingSequence[count..];
        
        result
            .Append(count.ToString())
            .Append(c);
    }

    return result.ToString();
}

(char, int) GetRun(ReadOnlySpan<char> sequence)
{
    char c = sequence[0];
    
    int count = 0;

    for (int i = 0;
         i < sequence.Length && sequence[i] == c;
         i++, count++)
    { }

    return (c, count);
}

