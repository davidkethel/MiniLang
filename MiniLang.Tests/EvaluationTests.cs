using MiniLang.Evaluator;
using MiniLang.Nodes;
using MiniLang.Parser;
using static MiniLang.Tests.TestUtils;

namespace MiniLang.Tests;

[TestClass]
public class EvaluationTests
{
    [TestMethod]
    public void TestConstantEvaluation()
    {
        Assert.AreEqual(new Value(1), Evaluate("1"));
        Assert.AreEqual(new Value(1.1m), Evaluate("1.1"));
        Assert.AreEqual(new Value("test"), Evaluate("\"test\""));
        Assert.AreEqual(new Value(true), Evaluate("true"));
        Assert.AreEqual(new Value(false), Evaluate("false"));
        Assert.AreEqual(Value.Undefined, Evaluate("undefined"));
        Assert.AreEqual(Value.Null, Evaluate("null"));
    }

    [TestMethod]
    public void TestVariableEvaluation()
    {
        Assert.AreEqual(new Value(1), Evaluate("a", new Dictionary<string, Value> { { "a", new Value(1) } }));
        Assert.AreEqual(Value.Undefined, Evaluate("b", new Dictionary<string, Value> { { "a", new Value(1) } }));
    }

    [TestMethod]
    public void TestCommentEvaluation()
    {
        Assert.AreEqual(Value.Undefined, Evaluate("com \"test\""));
    }

    [TestMethod]
    public void TestStatementListEvaluation()
    {
        Assert.AreEqual(new Value(5), Evaluate("1 * 2;2 + 4;3;4;5"));
    }

    [TestMethod]
    public void TestVariableDeclarationEvaluation()
    {
        var vars = new Dictionary<string, Value>();
        Assert.AreEqual(Value.Undefined, Evaluate("var a : int = 1", vars));
        Assert.AreEqual(new Value(1), vars["a"]);

        Assert.AreEqual(new Value(2), Evaluate("var b : int = 2;b", vars));
        Assert.AreEqual(new Value(1), vars["a"]);
        Assert.AreEqual(new Value(2), vars["b"]);
    }

    [TestMethod]
    public void TestVariableAssignmentEvaluation()
    {
        var vars = new Dictionary<string, Value>();
        Assert.AreEqual(Value.Undefined, Evaluate("var a : int = 2;set a = 1", vars));
        Assert.AreEqual(new Value(1), vars["a"]);

        Assert.AreEqual(Value.Undefined, Evaluate("set a = 2", vars));
        Assert.AreEqual(new Value(2), vars["a"]);

        Assert.AreEqual(new Value(3), Evaluate("var b : int = 2;set b = 3;b", vars));
        Assert.AreEqual(new Value(2), vars["a"]);
        Assert.AreEqual(new Value(3), vars["b"]);
    }

    [TestMethod]
    public void TestFunctionDeclarationEvaluation()
    {
        const string code = "fun test() : int { null }";
        var parser = new LanguageParser();
        var evaluator = new LanguageEvaluator();
        var context = new EvaluationContext();
        var expression = parser.Parse(code);
        var result = evaluator.Evaluate(expression, context);

        Assert.AreEqual(Value.Undefined, result);
        Assert.AreEqual("test", context.Functions["test"].Name);
        Assert.AreEqual(DataType.Integer, context.Functions["test"].ReturnType);
        Assert.AreEqual(0, context.Functions["test"].Parameters.Count);
        Assert.IsInstanceOfType(context.Functions["test"].Body, typeof(ConstantNode));
    }

    [TestMethod]
    public void TestFunctionCallEvaluation()
    {
        Assert.AreEqual(new Value(1), Evaluate("fun test() : int { 1 };call test()"));
        Assert.AreEqual(new Value(6), Evaluate("fun test(x : int) : int { x + 1 };call test(5)"));
    }

    [TestMethod]
    public void TestBinaryExpressionEvaluation()
    {
        Assert.AreEqual(new Value(10), Evaluate("5 * 2"));
        Assert.AreEqual(new Value(7), Evaluate("5 + 2"));
        Assert.AreEqual(new Value(13), Evaluate("5 * 2 + 3"));
        Assert.AreEqual(new Value(11), Evaluate("5 + 2 * 3"));
        Assert.AreEqual(new Value(true), Evaluate("100 - 1 < 10 * 10"));
        Assert.AreEqual(new Value(false), Evaluate("100 + 1 < 10 * 10"));
        Assert.AreEqual(new Value(2), Evaluate("fun test() : int { 1 };call test() + 1"));
        Assert.AreEqual(new Value(2), Evaluate("fun test() : int { 1 };1 + call test()"));
        Assert.AreEqual(new Value(3), Evaluate("fun test() : int { 1 };1 + call test() + 1"));
    }

    [TestMethod]
    public void TestIfEvaluation()
    {
        Assert.AreEqual(new Value(2), Evaluate("if (true) { 2 } else { 0 }"));
        Assert.AreEqual(Value.Undefined, Evaluate("if (false) { 2 }"));
        Assert.AreEqual(new Value(3), Evaluate("if (1 < 2) { 3 }"));
    }

    [TestMethod]
    public void TestWhileEvaluation()
    {
        Assert.AreEqual(new Value(10), Evaluate("var x : int = 0; while (x < 10) { set x = x + 1 }; x"));
        Assert.AreEqual(new Value(0), Evaluate("var x : int = 0; while (x > 10) { set x = x + 1 }; x"));
    }

    [TestMethod]
    public void TestFibExample()
    {
        const string fib = "fun fib(n : int) : int { if (n <= 1) { n } else { call fib(n-1) + call fib(n-2) } };";
        Assert.AreEqual(new Value(1), Evaluate(fib + "call fib(1)"));
        Assert.AreEqual(new Value(1), Evaluate(fib + "call fib(2)"));
        Assert.AreEqual(new Value(34), Evaluate(fib + "call fib(9)"));
    }
}