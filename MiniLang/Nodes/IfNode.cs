using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class IfNode : Node
{
    public Node Condition { get; set; }
    public Node Body { get; set; }
    public Node? ElseBody { get; set; }

    public IfNode(Token token, Node condition, Node body, Node? elseBody) : base(token)
    {
        Condition = condition;
        Body = body;
        ElseBody = elseBody;
    }
}