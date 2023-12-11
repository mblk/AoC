namespace aoc.common;

public static class CombinationUtils
{
    public static IEnumerable<(T, T)> GetCombinationPairs<T>(this IReadOnlyList<T> source)
    {
        for (var first = 0; first < source.Count; first++)
        {
            for (var second = first + 1; second < source.Count; second++)
            {
                yield return (source[first], source[second]);
            }
        }
    }
}