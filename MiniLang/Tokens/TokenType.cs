namespace MiniLang.Tokens;

public enum TokenType
{
    Invalid,
    Symbol,
    Name,
    String,
    Char,
    Number,
    Whitespace,
    Comment,
    Custom,
    End
}