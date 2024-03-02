using System.Globalization;

namespace MiniLang.Evaluator;

/// <summary>
/// Stores a runtime value and its associated type during language evaluation.
/// </summary>
public class Value
{
    private static readonly object UndefinedValue = new();
    private static readonly object NullValue = new();

    public static readonly Value Undefined = new Value(DataType.Undefined, UndefinedValue);
    public static readonly Value Null = new Value(DataType.Null, NullValue);

    public DataType Type { get; set; }
    public object ObjectValue { get; set; }

    public bool BooleanValue => (bool)ObjectValue;
    public string StringValue => (string)ObjectValue;
    public int IntValue => (int)ObjectValue;
    public decimal DecimalValue => (decimal)ObjectValue;

    public Value(DataType type, object? objectValue)
    {
        Type = type;

        if (Type == DataType.Null)
        {
            ObjectValue = NullValue;
            return;
        }

        if (Type == DataType.Undefined)
        {
            ObjectValue = UndefinedValue;
            return;
        }

        ValidateValue(type, objectValue);
        ObjectValue = objectValue!;
    }

    public Value(bool val) : this(DataType.Boolean, val) { }
    public Value(string val) : this(DataType.String, val) { }
    public Value(int val) : this(DataType.Integer, val) { }
    public Value(decimal val) : this(DataType.Decimal, val) { }

    private static void ValidateValue(DataType type, object? objectValue)
    {
        if (objectValue != null)
        {
            var ty = objectValue.GetType();
            if (ty != typeof(bool) &&
                ty != typeof(string) &&
                ty != typeof(int) &&
                ty != typeof(decimal))
            {
                throw new InvalidOperationException($"Unsupported value data type: {ty.Name}");
            }
        }
        else
        {
            throw new InvalidOperationException($"Unsupported null type: {type}");
        }
    }

    public void Set(object value)
    {
        if (Type == DataType.Null || Type == DataType.Undefined) throw new InvalidOperationException("Cannot set null/undefined");
        ValidateValue(Type, value);
        ObjectValue = value;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case DataType.Boolean:
                return BooleanValue.ToString(CultureInfo.InvariantCulture);
            case DataType.String:
                return '"' + StringValue + '"';
            case DataType.Integer:
                return IntValue.ToString(CultureInfo.InvariantCulture);
            case DataType.Decimal:
                return DecimalValue.ToString(CultureInfo.InvariantCulture);
            case DataType.Null:
                return "null";
            case DataType.Undefined:
                return "undefined";
            default:
                return $"{Type}[{ObjectValue}]";
        }
    }

    public bool Equals(Value other)
    {
        if (Type != other.Type) return false;
        switch (Type)
        {
            case DataType.Null:
            case DataType.Undefined:
                return true;
            case DataType.Boolean:
            case DataType.String:
            case DataType.Integer:
            case DataType.Decimal:
                return Object.Equals(ObjectValue, other.ObjectValue);
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Value)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)Type * 397) ^ (ObjectValue != null ? ObjectValue.GetHashCode() : 0);
        }
    }
}