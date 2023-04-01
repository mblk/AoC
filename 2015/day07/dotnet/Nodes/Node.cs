namespace day07.Nodes;

internal delegate uint ExpressionResolver(string expression);

internal abstract class Node
{
    public string OutputName { get; }
    
    protected Node(string outputName)
    {
        OutputName = outputName;
    }
    
    public abstract uint GetValue(ExpressionResolver r);
}