using MiniLang.Nodes;
using MiniLang.Parser;

namespace MiniLang.Tests;

[TestClass]
public class SimpleNodeTests
{
    private static Node Parse(string code)
    {
        var parser = new LanguageParser();
        return parser.Parse(code);
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("1")]
    [DataRow("1.1")]
    [DataRow("\"test\"")]
    [DataRow("true")]
    [DataRow("false")]
    [DataRow("null")]
    [DataRow("undefined")]
    public void TestConstant(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(ConstantNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("a")]
    [DataRow("a123")]
    public void TestVariable(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(VariableNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("2 > 1")]
    [DataRow("1 + 2")]
    [DataRow("1 + 2 + 3")]
    [DataRow("1 + 2 + 3 * 4")]
    public void TestBinaryExpression(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(BinaryExpressionNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("call test()")]
    [DataRow("call test(1)")]
    [DataRow("call test(1,2)")]
    [DataRow("call test(1 + 1)")]
    [DataRow("call test(aaa)")]
    public void TestFunctionCall(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(FunctionCallNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("fun test() : int { null }")]
    [DataRow("fun test(a : int) : int { null }")]
    [DataRow("fun test(a : int, b : str) : int { null }")]
    public void TestFunctionDeclaration(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(FunctionDeclarationNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("var test : int = 1")]
    [DataRow("var test : int = 1 + 2")]
    public void TestVariableDeclaration(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(VariableDeclarationNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("set test = 1")]
    [DataRow("set test = 1 + 2")]
    public void TestVariableAssignment(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(VariableAssignmentNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("if (true) { null }")]
    [DataRow("if (true) { null } else { null }")]
    public void TestIf(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(IfNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("while (true) { null }")]
    [DataRow("while (false) { null }")]
    public void TestWhile(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(WhileNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("com \"\"")]
    [DataRow("com \"this is a test\"")]
    public void TestComment(string code)
    {
        Assert.IsInstanceOfType(Parse(code), typeof(CommentNode));
    }

    [TestMethod]
    public void TestStatementList()
    {
        const string code = @"1;var x : int = 1; call test()";
        var node = Parse(code);
        Assert.IsInstanceOfType(node, typeof(StatementListNode));
        var sl = (StatementListNode)node;
        Assert.IsInstanceOfType(sl.Statements[0], typeof(ConstantNode));
        Assert.IsInstanceOfType(sl.Statements[1], typeof(VariableDeclarationNode));
        Assert.IsInstanceOfType(sl.Statements[2], typeof(FunctionCallNode));
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("var 1 : int = 1")]
    [DataRow("var a : int = @")]
    [DataRow("1 ** 2")]
    [DataRow("^")]
    public void TestInvalidSyntax(string code)
    {
        try
        {
            Parse(code);
        }
        catch
        {
            return; // good
        }
        Assert.Fail();
    }
}