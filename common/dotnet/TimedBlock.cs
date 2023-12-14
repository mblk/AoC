using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace aoc.common;

public class TimedBlock : IDisposable
{
    private readonly string _name;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public TimedBlock(string name)
    {
        _name = name;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        Console.WriteLine($"{_name}: {_stopwatch.ElapsedMilliseconds} ms");
    }

    public static T Time<T>(Func<T> action, [CallerArgumentExpression(nameof(action))] string name = "")
    {
        var sw = Stopwatch.StartNew();
        var r = action();
        sw.Stop();
        Console.WriteLine($"{name}: {sw.ElapsedMilliseconds} ms");
        return r;
    }
}