using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class FunctionCallNode : Node
{
    public string Name { get; set; }
    public List<Node> Parameters { get; set; }

    public FunctionCallNode(Token token, string name, List<Node> parameters) : base(token)
    {
        Name = name;
        Parameters = parameters;
    }
}