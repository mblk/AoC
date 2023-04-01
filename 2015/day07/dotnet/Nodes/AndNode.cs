namespace day07.Nodes;

internal class AndNode : BinaryNode
{
    public AndNode(string outputName, string leftInputExpression, string rightInputExpression)
        : base(outputName, leftInputExpression, rightInputExpression) { }
    
    public override uint GetValue(ExpressionResolver r) => GetLeftInputValue(r) & GetRightInputValue(r);
}