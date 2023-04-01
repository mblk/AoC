namespace day07.Nodes;

internal class NotNode : UnaryNode
{
    public NotNode(string outputName, string inputExpression) : base(outputName, inputExpression) { }

    public override uint GetValue(ExpressionResolver r) => GetInputValue(r) ^ 0xFFFFFFFF;
}