namespace day07.Nodes;

internal abstract class BinaryNode : Node
{
    private readonly string _leftInputExpression;
    private readonly string _rightInputExpression;
    
    protected BinaryNode(string outputName, string leftInputExpression, string rightInputExpression)
        :base(outputName)
    {
        _leftInputExpression = leftInputExpression;
        _rightInputExpression = rightInputExpression;
    }

    protected uint GetLeftInputValue(ExpressionResolver r) => r(_leftInputExpression);
    protected uint GetRightInputValue(ExpressionResolver r) => r(_rightInputExpression);
}