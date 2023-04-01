using System.Collections.ObjectModel;
using day07.Nodes;

namespace day07;

internal class Resolver
{
    private readonly Dictionary<string, uint> _cache = new();
    private readonly ReadOnlyDictionary<string, Node> _nodes;

    public Resolver(ReadOnlyDictionary<string, Node> nodes)
    {
        _nodes = nodes;
    }

    public void Override(string name, uint value)
    {
        _cache[name] = value;
    }
    
    public uint Resolve(string expression)
    {
        if (_cache.TryGetValue(expression, out uint value))
            return value;
        
        if (UInt32.TryParse(expression, out value))
            ;
        else if (_nodes.TryGetValue(expression, out var node))
            value = node.GetValue(Resolve);
        else
            throw new Exception($"Can't resolve expression '{expression}'");

        value &= 0xFFFF;
        _cache.Add(expression, value);
        return value;
    }
}