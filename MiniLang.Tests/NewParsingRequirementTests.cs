using MiniLang.Evaluator;
using static MiniLang.Tests.TestUtils;

namespace MiniLang.Tests;

[TestClass]
public class NewParsingRequirementTests
{
    /// <summary>
    /// Requirement: Add support for values to contain expressions enclosed in parentheses.
    /// The expressions in the parentheses will have higher precedence than expressions
    /// outside the parentheses.
    /// </summary>
    [TestMethod]
    public void TestGroupedOperations()
    {
        Assert.AreEqual(new Value(1), Evaluate("(1)"));
        Assert.AreEqual(new Value(1 + 2 * 3), Evaluate("1 + 2 * 3"));
        Assert.AreEqual(new Value((1 + 2) * 3), Evaluate("(1 + 2) * 3"));
        Assert.AreEqual(new Value(1 + (2 * 3)), Evaluate("1 + (2 * 3)"));
        Assert.AreEqual(new Value(1 + (2 * (4 - 3))), Evaluate("1 + (2 * (4 - 3))"));
    }

    /// <summary>
    /// Requirement: Add support for an empty statement when parsing a statement list.
    /// An empty statement will always return undefined.
    /// </summary>
    [TestMethod]
    public void TestEmptyStatement()
    {
        Assert.AreEqual(Value.Undefined, Evaluate(""));
        Assert.AreEqual(Value.Undefined, Evaluate("  "));
        Assert.AreEqual(Value.Undefined, Evaluate(";;"));
        Assert.AreEqual(Value.Undefined, Evaluate(" ;    ;;"));
        Assert.AreEqual(Value.Null, Evaluate(";null"));
        Assert.AreEqual(new Value(456), Evaluate(";;123;;456"));
        Assert.AreEqual(new Value(5), Evaluate(" ;    ;;  5"));
        Assert.AreEqual(Value.Undefined, Evaluate("if (true) {}"));
        Assert.AreEqual(new Value(1), Evaluate("if (false) {} else {1}"));
    }

    /// <summary>
    /// Requirement: Add new syntax to the "if" statement to allow "else if" chains to be added.
    /// Semantically, an else if is identical to having an "if" statment nested in the "else" block.
    /// Example:
    /// <code>
    ///     if (true) { 1 }
    ///     else if (false) { 2 }
    /// </code>
    /// This is identical to:
    /// <code>
    ///     if (true) { 1 }
    ///     else {
    ///         if (false) { 2 }
    ///     }
    /// </code>
    /// </summary>
    [TestMethod]
    public void TestElseIf()
    {
        Assert.AreEqual(new Value(1), Evaluate("if (true) { 1 } else if (true) { 2 }"));
        Assert.AreEqual(new Value(2), Evaluate("if (false) { 1 } else if (true) { 2 }"));
        Assert.AreEqual(new Value(2), Evaluate("if (false) { 1 } else if (true) { 2 } else { 3 }"));
    }

    /// <summary>
    /// NOTE: This requirement is more complicated than the others and will require changes to multiple files!
    /// ----
    /// Requirement: Add a new character data type with type keyword `char`, and with
    /// declaration syntax of a single character enclosed in single quotes, e.g. `'a'`.
    /// </summary>
    [TestMethod]
    public void TestCharacterType()
    {
        // todo: once char type added, replace this test with the commented code below
        // Assert.AreEqual(new Value('a'), Evaluate("'a'"));
        Assert.AreEqual("a", Evaluate("'a'").ObjectValue.ToString());
    }
}