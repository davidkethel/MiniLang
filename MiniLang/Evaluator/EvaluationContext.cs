using MiniLang.Nodes;

namespace MiniLang.Evaluator;

/// <summary>
/// Stores declared variables and functions during evaluation.
/// </summary>
public class EvaluationContext
{
    public Dictionary<string, Value> Variables { get; set; }
    public Dictionary<string, FunctionDeclarationNode> Functions { get; set; }
    
    public EvaluationContext()
    {
        Variables = new Dictionary<string, Value>();
        Functions = new Dictionary<string, FunctionDeclarationNode>();
    }
}