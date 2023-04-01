namespace day07.Nodes;

internal abstract class UnaryNode : Node
{
    private readonly string _inputExpression;
    
    protected UnaryNode(string outputName, string inputExpression)
        :base(outputName)
    {
        _inputExpression = inputExpression;
    }

    protected uint GetInputValue(ExpressionResolver r) => r(_inputExpression);
}