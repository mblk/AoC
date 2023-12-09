using System.Numerics;

namespace aoc.common;

public static class MathUtils
{
    public static T GreatestCommonDenominator<T>(T a, T b)
        where T : IBinaryInteger<T>
    {
        while (a != T.Zero)
        {
            var temp = a;
            a = b % a;
            b = temp;
        }
        return b;
    }

    public static T LeastCommonMultiple<T>(T a, T b)
        where T : IBinaryInteger<T>
    {
        return checked((a * b) / GreatestCommonDenominator(a, b));
    }

    public static T LeastCommonMultiple<T>(IEnumerable<T> values)
        where T : IBinaryInteger<T>
    {
        T temp = T.One;
        foreach (var value in values)
            temp = LeastCommonMultiple(temp, value);
        return temp;
    }
}