using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class VariableAssignmentNode : Node
{
    public string Name { get; set; }
    public Node Body { get; set; }

    public VariableAssignmentNode(Token token, string name, Node body) : base(token)
    {
        Name = name;
        Body = body;
    }
}