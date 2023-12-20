using System.Diagnostics;
using day19.Data;

namespace day19;

public class PartProcessor
{
    private readonly IReadOnlyList<Workflow> _workflows;

    public PartProcessor(IReadOnlyList<Workflow> workflows)
    {
        _workflows = workflows;
    }

    public IEnumerable<Part> ProcessParts(IEnumerable<Part> parts)
    {
        return parts.Where(ProcessPart);
    }

    private bool ProcessPart(Part part)
    {
        string currentWorkflowName = Workflow.StartingWorkflowName;

        while (true)
        {
            var workflow = _workflows.Single(w => w.Name == currentWorkflowName);

            foreach (var rule in workflow.Rules)
            {
                if (!EvaluateRule(rule, part))
                    continue;

                if (rule.Action == RuleAction.Redirect)
                {
                    currentWorkflowName = rule.NextWorkflow!;
                    break;
                }

                return rule.Action == RuleAction.Accept;
            }
        }
    }

    private static bool EvaluateRule(Rule rule, Part part)
    {
        return rule.Condition switch
        {
            RuleCondition.Always => true,
            RuleCondition.LessThan => part.GetValue(rule.Input) < rule.Value,
            RuleCondition.MoreThan => part.GetValue(rule.Input) > rule.Value,
            _ => throw new UnreachableException(),
        };
    }
}