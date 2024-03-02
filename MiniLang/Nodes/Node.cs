using MiniLang.Tokens;

namespace MiniLang.Nodes;

public abstract class Node
{
    public Token Token { get; }

    protected Node(Token token)
    {
        Token = token;
    }
}