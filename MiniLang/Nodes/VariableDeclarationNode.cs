using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class VariableDeclarationNode : Node
{
    public string Name { get; set; }
    public DataType Type { get; set; }
    public Node Body { get; set; }

    public VariableDeclarationNode(Token token, string name, DataType type, Node body) : base(token)
    {
        Name = name;
        Type = type;
        Body = body;
    }
}