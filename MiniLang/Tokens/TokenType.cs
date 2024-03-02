namespace MiniLang.Tokens;

public enum TokenType
{
    Invalid,
    Symbol,
    Name,
    String,
    Number,
    Whitespace,
    Comment,
    Custom,
    End
}