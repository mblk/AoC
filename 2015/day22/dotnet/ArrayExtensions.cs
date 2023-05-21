namespace day22;

internal static class ArrayExtensions
{
    public static T[] AddItemToArray<T>(this T[] source, T item)
    {
        var list = source.ToList();
        list.Add(item);
        return list.ToArray();
    }
}