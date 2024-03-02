using MiniLang.Evaluator;
using static MiniLang.Tests.TestUtils;

namespace MiniLang.Tests;

[TestClass]
public class NewEvaluationRequirementTests
{
    /// <summary>
    /// Requirement: math & comparison operations between integer and decimal types
    /// should be supported. Both operands are converted to decimals prior to
    /// the operation.
    /// </summary>
    [TestMethod]
    public void TestBinaryOperationsOnIntegerAndDecimals()
    {
        // Assert.AreEqual(new Value(2.1m), Evaluate("1 + 1.1"));
        Assert.AreEqual(new Value(2.1m), Evaluate("1.1 + 1"));
        // Assert.AreEqual(new Value(2m), Evaluate("1 * 2.0"));
        Assert.AreEqual(new Value(2m), Evaluate("1.0 * 2"));
        Assert.AreEqual(new Value(true), Evaluate("1.0 < 2"));
        // Assert.AreEqual(new Value(true), Evaluate("1 < 2.0"));
    }

    /// <summary>
    /// Requirement: Add evaluation-time type checking to ensure that functions return
    /// the type specified by their return type. Throw an exception if the evaluated type
    /// does not match the expected return type.
    /// </summary>
    [TestMethod]
    public void TestRuntimeFunctionTypeChecking()
    {
        try
        {
            Evaluate("fun test() : str { 1 }; call test()");
        }
        catch
        {
            return; // good
        }

        Assert.Fail();
    }

    /// <summary>
    /// Requirement: When evaluating a statement list, if a comment is the last
    /// statement in the list, it should not alter the return value (which should
    /// be the return value of the last non-comment statement in the list).
    /// </summary>
    [TestMethod]
    public void TestCommentsIgnoredDuringExecution()
    {
        Assert.AreEqual(new Value(1), Evaluate(@"1; com ""comment"""));
    }

    /// <summary>
    /// Requirement: When evaluating a statement list, if a comment is the last
    /// statement in the list, it should not alter the return value (which should
    /// be the return value of the last non-comment statement in the list).
    /// </summary>
    [TestMethod]
    public void TestMultipleCommentsIgnoredDuringExecution()
    {
        Assert.AreEqual(new Value(1), Evaluate(@"1; com ""comment""; com ""another comment"""));
    }

    /// <summary>
    /// Requirement: For logical operations (and/or), if the result is known before
    /// evaluating later expressions, then the later expressions are not evaluated.
    /// </summary>
    [TestMethod]
    public void TestShortCircuitingLogicalOperators()
    {
        Assert.AreEqual(new Value(false), Evaluate("false && call error()"));
        Assert.AreEqual(new Value(true), Evaluate("true || call error()"));
        Assert.AreEqual(new Value(false), Evaluate("true && false && call error()"));
    }
}