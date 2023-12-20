namespace day19.Data;

public record Workflow(string Name, IReadOnlyList<Rule> Rules)
{
    public const string StartingWorkflowName = "in";
}

public record Rule(PartValue Input, RuleCondition Condition, int Value, RuleAction Action, string? NextWorkflow);

public enum RuleCondition
{
    Always,
    MoreThan,
    LessThan,
}

public enum RuleAction
{
    Accept,
    Reject,
    Redirect,
}
