using System.Security.Cryptography;
using System.Text;

Console.WriteLine($"Part1: {FindAdventCoin(5)}");
Console.WriteLine($"Part2: {FindAdventCoin(6)}");

long FindAdventCoin(int minLeadingZeros)
{
    const string prefix = "ckczppom";
    using var md5 = MD5.Create();

    for (long i = 1;; i++)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes($"{prefix}{i}");
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        int leadingZeros = 0;
        foreach (var b in hashBytes)
        {
            if ((b & 0xF0) == 0) leadingZeros++;
            else break;
            if ((b & 0x0F) == 0) leadingZeros++;
            else break;
        }

        if (leadingZeros >= minLeadingZeros)
            return i;
    }
}
