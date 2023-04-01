namespace day07.Nodes;

internal class OrNode : BinaryNode
{
    public OrNode(string outputName, string leftInputExpression, string rightInputExpression)
        : base(outputName, leftInputExpression, rightInputExpression) { }
    
    public override uint GetValue(ExpressionResolver r) => GetLeftInputValue(r) | GetRightInputValue(r);
}