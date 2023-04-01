namespace day07.Nodes;

internal class WireNode : UnaryNode
{
    public WireNode(string outputName, string inputExpression) : base(outputName, inputExpression) { }
    
    public override uint GetValue(ExpressionResolver r) => GetInputValue(r);
}