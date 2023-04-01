namespace day07.Nodes;

internal class RightShiftNode : BinaryNode
{
    public RightShiftNode(string outputName, string leftInputExpression, string rightInputExpression)
        : base(outputName, leftInputExpression, rightInputExpression) { }
    
    public override uint GetValue(ExpressionResolver r) => GetLeftInputValue(r) >> (int)GetRightInputValue(r);
}