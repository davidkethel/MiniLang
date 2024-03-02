using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class StatementListNode : Node
{
    public List<Node> Statements { get; set; }

    public StatementListNode(Token token, List<Node> statements) : base(token)
    {
        Statements = statements;
    }
}