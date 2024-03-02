using MiniLang.Evaluator;
using MiniLang.Parser;

namespace MiniLang.Tests;

public static class TestUtils
{
    public static Value Evaluate(string code, Dictionary<string, Value>? variables = null)
    {
        var parser = new LanguageParser();
        var evaluator = new LanguageEvaluator();
        var context = new EvaluationContext();
        var expression = parser.Parse(code);
        if (variables != null)
        {
            foreach (var kv in variables)
            {
                context.Variables[kv.Key] = kv.Value;
            }
        }
        var result = evaluator.Evaluate(expression, context);
        if (variables != null)
        {
            foreach (var kv in context.Variables)
            {
                variables[kv.Key] = kv.Value;
            }
        }
        return result;
    }
}