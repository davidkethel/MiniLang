using MiniLang.Tokens;

namespace MiniLang.Nodes;

public class ConstantNode : Node
{
    private static readonly object UndefinedValue = new();
    private static readonly object NullValue = new();

    public DataType Type { get; }
    public object Value { get; }

    public ConstantNode(Token token, DataType type, object value) : base(token)
    {
        Type = type;
        Value = value;
        VerifyType();
    }

    public static ConstantNode Undefined(Token token) => new(token, DataType.Undefined, UndefinedValue);
    public static ConstantNode Null(Token token) => new(token, DataType.Null, NullValue);
    public static ConstantNode Boolean(Token token, bool value) => new(token, DataType.Boolean, value);
    public static ConstantNode Integer(Token token, int value) => new(token, DataType.Integer, value);
    public static ConstantNode Decimal(Token token, decimal value) => new(token, DataType.Decimal, value);
    public static ConstantNode String(Token token, string value) => new(token, DataType.String, value);
    public static ConstantNode Char(Token token, char value) => new(token, DataType.Char, value);

    private void VerifyType()
    {
        var valid = Type switch
        {
            DataType.Undefined => ReferenceEquals(Value, UndefinedValue),
            DataType.Null => ReferenceEquals(Value, NullValue),
            DataType.Boolean => Value is bool,
            DataType.Integer => Value is int,
            DataType.Decimal => Value is decimal,
            DataType.String => Value is string,
            DataType.Char => Value is char,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (!valid) throw new InvalidOperationException($"Constant type mismatch. Expected {Type}, got {Value.GetType().Name}.");
    }
}