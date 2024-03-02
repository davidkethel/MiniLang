# MiniLang

MiniLang is a basic expression evaluation language consisting of one or more statements
separated by semicolons (`;`). The final statement returns the value of the expression.

There are two projects in this solution:

- ThiessLang - implemenation of MiniLang
    - Evaluator - runtime for the language
    - Nodes - expression nodes for a concrete syntax tree
    - Parser - parser to convert a string into a syntax tree
    - Tokens - basic tokeniser, not important to know the details of this
- ThiessLang.Tests - unit tests for MiniLang
    - SimpleNodeTests - very basic tests for each leaf node in the language
    - EvaluationTests - very basic tests for each language construct
    - NewParsingRequirementTests - failing tests for parsing functionality that is not yet implemented
    - NewEvaluationRequirementTests - failing tests for evaulation functionality that is not yet implemented


## How to use this C# library

```cs
// Get the code to parse & evaluate
var code = "x + 1";

// Create instances of the parser & evaluator
var parser = new LanguageParser();
var evaluator = new LanguageEvaluator();

// Parse/compile the expression
var expression = parser.Parse(code);

// Create an evaluation context - this will hold all the variables used by the program
var context = new EvaluationContext();

// (optional) Add some input values to the context:
context.Variables["x"] = new Value(1); // sets x = 1 (int) during evaluation

// Evaluate the expression
var result = evaluator.Evaluate(expression, context);

// The result is the return value of the final statement in the expression.
// In this case since our expression is simply to return `x + 1`, and we
// provided `x = 1` for the input, we know the result will be `2`.

var two = result.IntValue; // two == 2
```

## Example programs

Basic hello world:

```
com "this program will return the string `Hello, World!`";
var x : str = "Hello, ";
var y : str = "World!";
x + y
```

All features in 1 program:

```
com "this is a comment";

com "declaring variables & types:";
var s : str = "string";
var i : int = 1;
var d : dec = 2.5;
var b : bool = false;

com "setting variables and binary operations:";
set s = s + " test";         com "`s` is now `stringtest`";
set i = 1 + 2;               com "`i` is now 3";
set d = d * 3.0;             com "`d` is now .75";
set b = d > 5.0;             com "`b` is now `true`";

com "declaring a function:";
fun add_one(x : int) : int {
    x + 1
};

com "calling a function:";
set i = call add_one(i);     com "`i` is now 4"

com "if/while:";
if (b) {
    set d = d / 3;           com "`d` is now 2.5"
};
while (i > 0) {
    set i = i - 1
}
```

Return 10th Fibonacci number:

```
fun fib(n : int) : int {
    if (n <= 1) {
        n
    } else {
        call fib(n-1) + call fib(n-2)
    }
};
call fib(10)
```

## Data types

- Constants
    - Undefined (`undefined`)
    - Null (`null`)
- Types
    - `bool` - Boolean (`true`, `false`)
    - `int` - Integer (`123`)
    - `dec` - Decimal (`123.456`)
    - `str` - String (`"string"`)

## Reserved keywords

- Data types: `bool`, `int`, `dec`, `str`
- Constants: `undefined`, `null`, `true`, `false`
- Declarations: `var`, `fun`, `set`, `com`
- Control structures: `if`, `while`, `call`

## Language structures

- Declare a variable: `var x : int = 1;`
- Assign a variable: `set x = x + 1`
- If statement (else is optional): `if (true) { 1 } else { 2 }`
- While statement: `while (true) { 1 }`
- Declare a function: `fun fib(n : int) : int { if (n <= 1) { n } else { call fib(n-1) + call fib(n-2) } }`
- Call a function: `call fib(10)`
- Comment: `com "comment text here"`

## Binary operators

- Precedence 5 (highest)
    - multiply `*`
    - divide `/`
- Precedence 4
    - add `+`
    - subtract `-`
- Precedence 3
    - less `<`
    - less or equal `<=`
    - greater `>`
    - greater or equal `>=`
- Precedence 2
    - equal `==`
    - not equal `!=`
- Precedence 1
    - logical and `&&`
- Precedence 0 (lowest)
    - logical or `||`

Valid operators per type:

- all: equal, not equal
- `bool`: logical and, logical or
- `int`: multiply, divide, add, subtract, less, greater
- `dec`: multiply, divide, add, subtract, less, greater
- `str`: add (concatenate strings)

## Language grammar (for extra information only)

For information purposes, an ANTLR grammar of the language is below (note this grammar does not encode operator precedence):

```
grammar MiniLang;

ID: [a-zA-Z_][a-zA-Z_0-9]* ;
INT : [0-9]+ ;
STRING: '"' (~('"' | '\\'))* '"';
DECIMAL: INT '.' INT;
WS: [ \t\n\r\f]+ -> skip ;

type: 'null' | 'bool' | 'int' | 'dec' | 'str';
constant: 'undefined' | 'null' | 'true' | 'false' | INT | STRING | DECIMAL;
variable: ID;
function: ID;

operator
    : '*' | '/' | '+' | '-'
    | '<' | '<=' | '>' | '>=' | '==' | '!='
    | '&&' | '||'
    ;

comment: 'com' STRING;

program: statement_list EOF;

statement_list
    : statement (';' statement)*;

statement
    : fun_decl
    | var_decl
    | assignment
    | expression
    | while
    | if
    | comment
    ;
    
fun_decl
    : 'fun' function '(' (param (',' param)*)? ')' ':' type '{' statement_list '}'
    ;
    
param
    : variable ':' type
    ;
    
var_decl
    : 'var' variable ':' type '=' expression
    ;
    
assignment
    : 'set' variable '=' expression
    ;

value
    : constant
    | variable
    | call
    ;
    
expression
    : value (operator expression)?
    ;

call
    : 'call' function '(' (expression (',' expression)*)? ')'
    ;

while
    : 'while' '(' expression ')' '{' statement_list '}'
    ;

if
    : 'if' '(' expression ')' '{' statement_list '}' else?
    ;

else
    : 'else' '{' statement_list '}'
    ;

```