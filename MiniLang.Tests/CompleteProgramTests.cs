using MiniLang.Evaluator;
using static MiniLang.Tests.TestUtils;

namespace MiniLang.Tests;

[TestClass]
public class CompleteProgramTests
{
    [TestMethod]
    public void TestHelloWorld()
    {
        const string program = @"
            com ""this program will return the string `Hello, World!`"";
            var x : str = ""Hello, "";
            var y : str = ""World!"";
            x + y
        ";
        Assert.AreEqual(new Value("Hello, World!"), Evaluate(program));
    }

    [TestMethod]
    public void TestAllFeatures()
    {
        const string program = @"
            com ""this is a comment"";

            com ""declaring variables & types:"";
            var s : str = ""string"";
            var i : int = 1;
            var d : dec = 2.5;
            var b : bool = false;

            com ""setting variables and binary operations:"";
            set s = s + "" test"";         com ""`s` is now `stringtest`"";
            set i = 1 + 2;               com ""`i` is now 3"";
            set d = d * 3.0;             com ""`d` is now .75"";
            set b = d > 5.0;             com ""`b` is now `true`"";

            com ""declaring a function:"";
            fun add_one(x : int) : int {
                x + 1
            };

            com ""calling a function:"";
            set i = call add_one(i);     com ""`i` is now 4""

            com ""if/while:"";
            if (b) {
                set d = d / 3;           com ""`d` is now 2.5""
            };
            while (i > 0) {
                set i = i - 1
            }
        ";
        Assert.AreEqual(Value.Undefined, Evaluate(program));
    }

    [DataTestMethod]
    [DataRow(0, 0)]
    [DataRow(1, 1)]
    [DataRow(2, 1)]
    [DataRow(3, 2)]
    [DataRow(4, 3)]
    [DataRow(5, 5)]
    [DataRow(6, 8)]
    [DataRow(7, 13)]
    [DataRow(8, 21)]
    [DataRow(9, 34)]
    [DataRow(10, 55)]
    [DataRow(11, 89)]
    [DataRow(12, 144)]
    public void TestFibonacci(int n, int expected)
    {
        const string program = @"
                fun fib(n : int) : int {
                    if (n <= 1) {
                        n
                    } else {
                        call fib(n-1) + call fib(n-2)
                    }
                };
                call fib(x)
            ";
        var dict = new Dictionary<string, Value>
        {
            { "x", new Value(n) }
        };
        var actual = Evaluate(program, dict);
        Assert.AreEqual(new Value(expected), actual);
    }
}