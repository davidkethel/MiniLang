using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class CommentNode : Node
{
    public string Comment { get; set; }

    public CommentNode(Token token, string comment) : base(token)
    {
        Comment = comment;
    }
}