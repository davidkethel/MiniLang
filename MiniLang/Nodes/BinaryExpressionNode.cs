using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class BinaryExpressionNode : Node
{
    public OperatorType Operator { get; }
    public Node Left { get; }
    public Node Right { get; }

    public BinaryExpressionNode(Token token, OperatorType op, Node left, Node right) : base(token)
    {
        Operator = op;
        Left = left;
        Right = right;
    }
}