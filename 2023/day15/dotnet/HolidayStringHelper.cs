namespace day15;

public static class HolidayStringHelper
{
    public static byte Hash(string input)
    {
        byte h = 0;
        for (var i = 0; i < input.Length; i++)
        {
            h += (byte)input[i];
            h *= 17;
        }
        return h;
    }
}