using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class VariableNode : Node
{
    public string Name { get; }

    public VariableNode(Token token, string name) : base(token)
    {
        Name = name;
    }
}