using MiniLang.Nodes;

namespace MiniLang.Evaluator;

/// <summary>
/// Evaluator for the language
/// </summary>
public class LanguageEvaluator
{
    public Value Evaluate(Node node, EvaluationContext context)
    {
        if (node is BinaryExpressionNode ben) return EvaluateBinaryExpression(ben, context);
        if (node is CommentNode com) return EvaluateComment(com, context);
        if (node is ConstantNode con) return EvaluateConstant(con, context);
        if (node is FunctionCallNode fcn) return EvaluateFunctionCall(fcn, context);
        if (node is FunctionDeclarationNode fdn) return EvaluateFunctionDeclaration(fdn, context);
        if (node is IfNode ifn) return EvaluateIf(ifn, context);
        if (node is StatementListNode sln) return EvaluateStatementList(sln, context);
        if (node is VariableAssignmentNode van) return EvaluateVariableAssignment(van, context);
        if (node is VariableDeclarationNode vdn) return EvaluateVariableDeclaration(vdn, context);
        if (node is VariableNode vrn) return EvaluateVariable(vrn, context);
        if (node is WhileNode whn) return EvaluateWhile(whn, context);
        throw new ArgumentException($"Unknown node type {node.GetType().Name}", nameof(node));
    }

    private Value EvaluateBinaryExpression(BinaryExpressionNode node, EvaluationContext context)
    {
        var left = Evaluate(node.Left, context);
        var right = Evaluate(node.Right, context);
        if (left.Type != right.Type) throw new InvalidOperationException($"Cannot perform operations on mixed types. Left side is {left.Type}, right side is {right.Type}");
        var type = left.Type;
        switch (node.Operator)
        {
            case OperatorType.Add:
                if (type == DataType.Decimal) return new Value(left.DecimalValue + right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue + right.IntValue);
                if (type == DataType.String) return new Value(left.StringValue + right.StringValue);
                break;
            case OperatorType.Subtract:
                if (type == DataType.Decimal) return new Value(left.DecimalValue - right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue - right.IntValue);
                break;
            case OperatorType.Multiply:
                if (type == DataType.Decimal) return new Value(left.DecimalValue * right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue * right.IntValue);
                break;
            case OperatorType.Divide:
                if (type == DataType.Decimal) return new Value(left.DecimalValue / right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue / right.IntValue);
                break;
            case OperatorType.LogicalAnd:
                if (type == DataType.Boolean) return new Value(left.BooleanValue && right.BooleanValue);
                break;
            case OperatorType.LogicalOr:
                if (type == DataType.Boolean) return new Value(left.BooleanValue || right.BooleanValue);
                break;
            case OperatorType.LessThan:
                if (type == DataType.Decimal) return new Value(left.DecimalValue < right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue < right.IntValue);
                break;
            case OperatorType.LessThanEqual:
                if (type == DataType.Decimal) return new Value(left.DecimalValue <= right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue <= right.IntValue);
                break;
            case OperatorType.GreaterThan:
                if (type == DataType.Decimal) return new Value(left.DecimalValue > right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue > right.IntValue);
                break;
            case OperatorType.GreaterThanEqual:
                if (type == DataType.Decimal) return new Value(left.DecimalValue >= right.DecimalValue);
                if (type == DataType.Integer) return new Value(left.IntValue >= right.IntValue);
                break;
            case OperatorType.Equal:
                return new Value(left.Equals(right));
            case OperatorType.NotEqual:
                return new Value(!left.Equals(right));
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new InvalidOperationException($"Cannot perform operation {node.Operator} on type {type}");
    }

    private Value EvaluateComment(CommentNode com, EvaluationContext context)
    {
        // comments do nothing
        return Value.Undefined;
    }

    private Value EvaluateConstant(ConstantNode con, EvaluationContext context)
    {
        return new Value(con.Type, con.Value);
    }

    private Value EvaluateFunctionCall(FunctionCallNode fcn, EvaluationContext context)
    {
        var name = fcn.Name;
        if (!context.Functions.ContainsKey(name)) throw new InvalidOperationException($"Function not declared: {name}");

        var fun = context.Functions[name];
        if (fun.Parameters.Count != fcn.Parameters.Count) throw new InvalidOperationException($"Function parameter count incorrect for {name}: expected {fun.Parameters.Count}, got {fcn.Parameters.Count}");

        // functions run in a pure context, no external scope is captured
        var subcontext = new EvaluationContext();
        for (var i = 0; i < fun.Parameters.Count; i++)
        {
            var par = fun.Parameters[i];
            var val = Evaluate(fcn.Parameters[i], context);
            if (par.Item2 != val.Type) throw new InvalidOperationException($"Incorrect parameter type for {name}({par.Item1}): Expected {par.Item2}, got {val.Type}");
            subcontext.Variables[par.Item1] = val;
        }

        // functions from the parent scope are passed  through
        foreach (var kv in context.Functions) subcontext.Functions[kv.Key] = kv.Value;

        return Evaluate(fun.Body, subcontext);
    }

    private Value EvaluateFunctionDeclaration(FunctionDeclarationNode fdn, EvaluationContext context)
    {
        var name = fdn.Name;
        if (context.Functions.ContainsKey(name)) throw new InvalidOperationException($"Function already declared: {name}");
        context.Functions.Add(name, fdn);
        return Value.Undefined;
    }

    private Value EvaluateIf(IfNode ifn, EvaluationContext context)
    {
        var cond = Evaluate(ifn.Condition, context);
        if (cond.Type != DataType.Boolean) throw new InvalidOperationException("Condition of if statement must return a boolean.");
        if (cond.BooleanValue) return Evaluate(ifn.Body, context);
        if (ifn.ElseBody != null) return Evaluate(ifn.ElseBody, context);
        return Value.Undefined;
    }

    private Value EvaluateStatementList(StatementListNode sln, EvaluationContext context)
    {

        var ret = Value.Undefined;
        foreach (var st in sln.Statements.Where(statement => statement is not CommentNode))
        {
            ret = Evaluate(st, context);
        }
        return ret;
    }

    private Value EvaluateVariableAssignment(VariableAssignmentNode van, EvaluationContext context)
    {
        var name = van.Name;
        if (!context.Variables.ContainsKey(name)) throw new InvalidOperationException($"Variable not declared: {name}");
        var variable = context.Variables[name];
        var value = Evaluate(van.Body, context);
        if (value.Type != variable.Type) throw new InvalidOperationException($"Invalid variable type for {name}: Expected {variable.Type}, got {value.Type}");
        variable.Set(value.ObjectValue);
        return Value.Undefined;
    }

    private Value EvaluateVariableDeclaration(VariableDeclarationNode vdn, EvaluationContext context)
    {
        var name = vdn.Name;
        if (context.Variables.ContainsKey(name)) throw new InvalidOperationException($"Variable already declared: {name}");
        var value = Evaluate(vdn.Body, context);
        if (value.Type != vdn.Type) throw new InvalidOperationException($"Invalid variable type for {name}: Expected {vdn.Type}, got {value.Type}");
        context.Variables.Add(name, value);
        return Value.Undefined;
    }

    private Value EvaluateVariable(VariableNode vrn, EvaluationContext context)
    {
        return context.Variables.TryGetValue(vrn.Name, out var v) ? v : Value.Undefined;
    }

    private Value EvaluateWhile(WhileNode whn, EvaluationContext context)
    {
        var ret = Value.Undefined;

        while (true)
        {
            var cond = Evaluate(whn.Condition, context);
            if (cond.Type != DataType.Boolean) throw new InvalidOperationException("Condition of while statement must return a boolean.");
            if (!cond.BooleanValue) break;
            ret = Evaluate(whn.Body, context);
        }

        return ret;
    }
}