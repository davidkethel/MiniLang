using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class WhileNode : Node
{
    public Node Condition { get; set; }
    public Node Body { get; set; }

    public WhileNode(Token token, Node condition, Node body) : base(token)
    {
        Condition = condition;
        Body = body;
    }
}