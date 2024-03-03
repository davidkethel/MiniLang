using MiniLang.Nodes;
using MiniLang.Tokens;
using MiniLang.Tokens.Readers;
using static MiniLang.Tokens.TokenParsing;

namespace MiniLang.Parser;

/// <summary>
/// Parser for the language. Converts text into a concrete syntax tree.
/// </summary>
public class LanguageParser
{
    private static readonly char[] ValidSymbols = new[]
    {
        // quotes
        Symbols.DoubleQuote,
        // operators
        Symbols.Minus,
        Symbols.Plus,
        Symbols.Star,
        Symbols.Slash,
        Symbols.Less,
        Symbols.Greater,
        Symbols.Equal,
        Symbols.Bang,
        Symbols.Pipe,
        Symbols.Ampersand,
        // definitions
        Symbols.OpenBrace,
        Symbols.CloseBrace,
        Symbols.OpenParen,
        Symbols.CloseParen,
        Symbols.Colon,
        Symbols.Comma,
        // terminator
        Symbols.Semicolon,
    };

    private static readonly Tokeniser Tokeniser = new(
        new StringTokenReader('"'),
        new UnsignedDecimalTokenReader(),
        new SymbolTokenReader(ValidSymbols),
        new NameTokenReader()
    );

    public Node Parse(string code)
    {
        var tokens = Tokeniser.Tokenise(code);
        using var it = tokens.GetEnumerator();
        it.MoveNext();
        return ParseStatementList(it);
    }

    /// <summary>
    /// Parse a list of statements, separated by semicolons
    /// </summary>
    /// <example>
    /// 1;2;3
    /// </example>
    private static Node ParseStatementList(IEnumerator<Token> it)
    {
        var token = it.Current;
        var list = new List<Node>
        {
            ParseStatement(it)
        };
        while (it.Current.Is(TokenType.Symbol, ";"))
        {
            Expect(it, TokenType.Symbol, ";");
            list.Add(ParseStatement(it));
        }

        return list.Count == 1 ? list[0] : new StatementListNode(token, list);
    }

    /// <summary>
    /// Parse a single statement
    /// </summary>
    private static Node ParseStatement(IEnumerator<Token> it)
    {
        // Handle an empty statement and return an Undefined Node
        if (it.Current.Is(TokenType.Symbol, ";")
            || it.Current.Is(TokenType.End)
            || it.Current.Is(TokenType.Symbol, "}"))
        {
            return ConstantNode.Undefined(it.Current);
        }

        if (it.Current.Is(TokenType.Name, "fun")) return ParseFunctionDeclaration(it);
        if (it.Current.Is(TokenType.Name, "var")) return ParseVariableDeclaration(it);
        if (it.Current.Is(TokenType.Name, "while")) return ParseWhile(it);
        if (it.Current.Is(TokenType.Name, "if")) return ParseIf(it);
        if (it.Current.Is(TokenType.Name, "com")) return ParseComment(it);
        if (it.Current.Is(TokenType.Name, "set")) return ParseAssignment(it);
        return ParseExpression(it);
    }

    /// <summary>
    /// Parse a function declaration
    /// </summary>
    /// <example>
    /// fun example(param1 : int) : int { param1 + 1 }
    /// </example>
    private static Node ParseFunctionDeclaration(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "fun");
        var name = Expect(it, TokenType.Name).Value;
        Expect(it, TokenType.Symbol, "(");
        var pars = new List<(string, DataType)>();
        var first = true;
        while (!it.Current.Is(TokenType.Symbol, ")"))
        {
            if (!first) Expect(it, TokenType.Symbol, ",");
            var pname = Expect(it, TokenType.Name).Value;
            Expect(it, TokenType.Symbol, ":");
            var ptype = ParseType(it);
            pars.Add((pname, ptype));
            first = false;
        }
        Expect(it, TokenType.Symbol, ")");
        Expect(it, TokenType.Symbol, ":");
        var type = ParseType(it);
        Expect(it, TokenType.Symbol, "{");
        var body = ParseStatementList(it);
        Expect(it, TokenType.Symbol, "}");
        return new FunctionDeclarationNode(tok, name, pars, type, body);
    }

    /// <summary>
    /// Parse a variable declaration
    /// </summary>
    /// <example>
    /// var example : int = 1;
    /// </example>
    private static Node ParseVariableDeclaration(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "var");
        var name = Expect(it, TokenType.Name).Value;
        Expect(it, TokenType.Symbol, ":");
        var type = ParseType(it);
        Expect(it, TokenType.Symbol, "=");
        var body = ParseExpression(it);
        return new VariableDeclarationNode(tok, name, type, body);
    }

    /// <summary>
    /// Parse a while loop
    /// </summary>
    /// <example>
    /// while (x != 10) { set x = x + 1 }
    /// </example>
    private static Node ParseWhile(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "while");
        Expect(it, TokenType.Symbol, "(");
        var condition = ParseExpression(it);
        Expect(it, TokenType.Symbol, ")");
        Expect(it, TokenType.Symbol, "{");
        var body = ParseStatementList(it);
        Expect(it, TokenType.Symbol, "}");
        return new WhileNode(tok, condition, body);
    }

    /// <summary>
    /// Parse an if statement (with optional else)
    /// </summary>
    /// <example>
    /// if (x != 10) { set x = x + 1 } else { set x = x - 1 }
    /// </example>
    private static Node ParseIf(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "if");
        Expect(it, TokenType.Symbol, "(");
        var condition = ParseExpression(it);
        Expect(it, TokenType.Symbol, ")");
        Expect(it, TokenType.Symbol, "{");
        var body = ParseStatementList(it);
        Expect(it, TokenType.Symbol, "}");
        Node? elseBody = null;
        if (it.Current.Is(TokenType.Name, "else"))
        {
            Expect(it, TokenType.Name, "else");
            if (it.Current.Is(TokenType.Name, "if"))
            {
                elseBody = ParseIf(it);
            }
            else
            {
                Expect(it, TokenType.Symbol, "{");
                elseBody = ParseStatementList(it);
                Expect(it, TokenType.Symbol, "}");
            }
        }
        return new IfNode(tok, condition, body, elseBody);
    }

    /// <summary>
    /// Parse a comment
    /// </summary>
    /// <example>
    /// com "this is a comment"
    /// </example>
    private static Node ParseComment(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "com");
        var text = Expect(it, TokenType.String).Value;
        return new CommentNode(tok, text);
    }

    /// <summary>
    /// Parse a variable assignment
    /// </summary>
    /// <example>
    /// set example = 1
    /// </example>
    private static Node ParseAssignment(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "set");
        var name = Expect(it, TokenType.Name).Value;
        Expect(it, TokenType.Symbol, "=");
        var body = ParseExpression(it);
        return new VariableAssignmentNode(tok, name, body);
    }

    /// <summary>
    /// Parse a an expression that is enclosed in parentheses.
    /// </summary>
    /// <example>
    /// "(4)"
    /// </example>
    private static Node ParseParentheses(IEnumerator<Token> it)
    {
        Expect(it, TokenType.Symbol, "(");
        var expression = ParseExpression(it);
        Expect(it, TokenType.Symbol, ")");

        return expression;
    }

    /// <summary>
    /// Parse an expression
    /// </summary>
    /// <example>
    /// 1 + 2
    /// </example>
    private static Node ParseExpression(IEnumerator<Token> it)
    {

        var chain = new List<(OperatorType? op, Node node)>
        {
            (null, ParseValue(it))
        };

        while (it.Current.Type == TokenType.Symbol)
        {
            var op = TryParseOperator(it.Current, it);
            if (op == null) break;

            chain.Add((op.Value, ParseValue(it)));
        }

        if (chain.Count == 1) return chain[0].node;
        return BuildOperatorTree(chain);
    }

    /// <summary>
    /// Parse a value
    /// </summary>
    /// <example>
    /// 1.2
    /// </example>
    private static Node ParseValue(IEnumerator<Token> it)
    {
        if (it.Current.Is(TokenType.Name, "call")) return ParseFunctionCall(it);

        if (it.Current.Is(TokenType.Symbol, "(")) return ParseParentheses(it);

        var tok = it.Current;
        it.MoveNext();
        if (tok.Is(TokenType.Number))
        {
            if (tok.Value.Contains('.')) return ConstantNode.Decimal(tok, decimal.Parse(tok.Value));
            return ConstantNode.Integer(tok, int.Parse(tok.Value));
        }
        if (tok.Is(TokenType.String)) return ConstantNode.String(tok, tok.Value);
        if (tok.Is(TokenType.Name, "true")) return ConstantNode.Boolean(tok, true);
        if (tok.Is(TokenType.Name, "false")) return ConstantNode.Boolean(tok, false);
        if (tok.Is(TokenType.Name, "null")) return ConstantNode.Null(tok);
        if (tok.Is(TokenType.Name, "undefined")) return ConstantNode.Undefined(tok);
        if (tok.Is(TokenType.Name)) return new VariableNode(tok, tok.Value);
        throw new InvalidOperationException($"Unexpected value token: {tok.Type}[{tok.Value}]");
    }

    /// <summary>
    /// Parse a function call
    /// </summary>
    /// <example>
    /// call example(1, 2)
    /// </example>
    private static Node ParseFunctionCall(IEnumerator<Token> it)
    {
        var tok = it.Current;
        Expect(it, TokenType.Name, "call");
        var name = Expect(it, TokenType.Name).Value;
        Expect(it, TokenType.Symbol, "(");
        var pars = new List<Node>();
        var first = true;
        while (!it.Current.Is(TokenType.Symbol, ")"))
        {
            if (!first) Expect(it, TokenType.Symbol, ",");
            pars.Add(ParseExpression(it));
            first = false;
        }
        Expect(it, TokenType.Symbol, ")");
        return new FunctionCallNode(tok, name, pars);
    }

    /// <summary>
    /// Parse a data type (must be a known type)
    /// </summary>
    /// <example>
    /// str
    /// </example>
    private static DataType ParseType(IEnumerator<Token> it)
    {
        var tok = it.Current.Value;
        it.MoveNext();
        return tok switch
        {
            "bool" => DataType.Boolean,
            "int" => DataType.Integer,
            "dec" => DataType.Decimal,
            "str" => DataType.String,
            _ => throw new InvalidOperationException($"Unknown type: {tok}")
        };
    }

    /// <summary>
    /// Collection used to sort operator precedence
    /// </summary>
    private static readonly Dictionary<OperatorType, int> Precedence = new()
    {
        { OperatorType.Multiply, 5 },
        { OperatorType.Divide, 5 },

        { OperatorType.Add, 4 },
        { OperatorType.Subtract, 4 },

        { OperatorType.LessThan, 3 },
        { OperatorType.LessThanEqual, 3 },
        { OperatorType.GreaterThan, 3 },
        { OperatorType.GreaterThanEqual, 3 },

        { OperatorType.Equal, 2 },
        { OperatorType.NotEqual, 2 },

        { OperatorType.LogicalAnd, 1 },
        { OperatorType.LogicalOr, 0 },
    };

    /// <summary>
    /// Builds an operator tree out of a chain of nodes and binary operators
    /// </summary>
    /// <param name="nodes">The list of nodes in the chain and their operators. The first node must have null for its operator (as it is the beginning of the chain)</param>
    /// <returns>A node representing the chain of binary operators with highest precedence operations pushed down to the lowest possible level</returns>
    /// <remarks>
    /// The nodes collection is a crude way to represent a chain of binary expressions.
    /// As an example, consider the following expression: `1 + x * 7 / 2 > 7`
    /// The nodes chain would be represented as the following:
    /// (null, 1), (+, x), (*, 7), (/, 2), (>, 7)
    /// We then want to build the tree based on precedence, resulting in implicit groups being formed: (1 + ((x * 7) / 2)) > 7
    /// Which gives us a tree looking something like:
    /// returnValue = binaryop(>, l1, r1)
    /// r1 = constant(7)
    /// l1 = binaryop(+, constant(1), r2)
    /// r2 = binaryop(/, l2, constant(2))
    /// l2 = binaryop(*, variable(x), constant(7))
    /// During evaluation, this results in l2 being evaluated as the first binaryop, since to evaluate the returnValue node,
    /// we must evaluate l1, and to evaluate l1 we must evaluate r2, and to evaluate r2 we must first evaluate l2.
    /// </remarks>
    private static Node BuildOperatorTree(List<(OperatorType? op, Node node)> nodes)
    {
        // classify all the nodes
        var classified = nodes.Select(x => (x.op.HasValue ? Precedence[x.op.Value] : -1, x.op, x.node)).ToList();
        return BuildOperatorTreeRecursive(classified);
    }

    /// <summary>
    /// Recursively build a tree from an ordered list of binary expressions.
    /// the first node MUST have a null operator.
    /// </summary>
    private static Node BuildOperatorTreeRecursive(List<(int precedence, OperatorType? op, Node node)> nodes)
    {
        if (nodes[0].op.HasValue) throw new InvalidOperationException();

        if (nodes.Count == 1) return nodes.First().node;
        if (nodes.Count == 2) return new BinaryExpressionNode(nodes[0].node.Token, nodes[1].op.GetValueOrDefault(), nodes[0].node, nodes[1].node);

        var lowestPrec = nodes.Where(x => x.op.HasValue).MinBy(x => x.precedence).precedence;
        var index = nodes.FindLastIndex(x => x.precedence == lowestPrec);

        var left = nodes.GetRange(0, index);

        var right = nodes.GetRange(index, nodes.Count - index);
        if (!right[0].op.HasValue) throw new InvalidOperationException();
        var op = right[0].op!.Value;
        right[0] = (-1, null, right[0].node);

        return new BinaryExpressionNode(left[0].node.Token, op, BuildOperatorTreeRecursive(left), BuildOperatorTreeRecursive(right));
    }

    /// <summary>
    /// Try and get a binary operator from the token stream. Moves the stream forward if one is found, otherwise returns null.
    /// </summary>
    private static OperatorType? TryParseOperator(Token token, IEnumerator<Token> it)
    {
        string? next;
        // only moves next if an operator is found
        switch (token.Value)
        {
            case "+":
                Next();
                return OperatorType.Add;
            case "-":
                Next();
                return OperatorType.Subtract;
            case "*":
                Next();
                return OperatorType.Multiply;
            case "/":
                Next();
                return OperatorType.Divide;
            case "&":
                Next();
                Expect(it, TokenType.Symbol, "&");
                return OperatorType.LogicalAnd;
            case "|":
                Next();
                Expect(it, TokenType.Symbol, "|");
                return OperatorType.LogicalOr;
            case "<":
                Next();
                if (NextIs("=")) return OperatorType.LessThanEqual;
                return OperatorType.LessThan;
            case ">":
                Next();
                if (NextIs("=")) return OperatorType.GreaterThanEqual;
                return OperatorType.GreaterThan;
            case "!":
                Next();
                Expect(it, TokenType.Symbol, "=");
                return OperatorType.NotEqual;
            case "=":
                Next();
                Expect(it, TokenType.Symbol, "=");
                return OperatorType.Equal;
        }

        return null;

        void Next()
        {
            it.MoveNext();
            var nt = it.Current;
            if (nt == null || nt.Type != TokenType.Symbol) next = null;
            else next = nt.Value;
        }

        bool NextIs(string str)
        {
            if (next != str) return false;
            it.MoveNext();
            return true;
        }
    }
}