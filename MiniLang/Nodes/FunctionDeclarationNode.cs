using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class FunctionDeclarationNode : Node
{
    public string Name { get; set; }
    public List<(string, DataType)> Parameters { get; set; }
    public DataType ReturnType { get; set; }
    public Node Body { get; set; }

    public FunctionDeclarationNode(Token token, string name, List<(string, DataType)> parameters, DataType returnType, Node body) : base(token)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }
}