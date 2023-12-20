using System.Diagnostics;
using day19.Data;

namespace day19;

public class PartRangeProcessor
{
    private readonly IReadOnlyList<Workflow> _workflows;

    public PartRangeProcessor(IReadOnlyList<Workflow> workflows)
    {
        _workflows = workflows;
    }

    public IEnumerable<PartRange> ProcessPartRange(PartRange startPartRange)
    {
        var openList = new Queue<(string, PartRange)>();
        openList.Enqueue((Workflow.StartingWorkflowName, startPartRange));

        while (openList.Count != 0)
        {
            var (workflowName, remainingPartRange) = openList.Dequeue();
            var workflow = _workflows.Single(w => w.Name == workflowName);

            foreach (var rule in workflow.Rules)
            {
                (PartRange? trueRange, PartRange? falseRange) = EvaluateRule(rule, remainingPartRange);

                if (trueRange.HasValue)
                {
                    switch (rule.Action)
                    {
                        case RuleAction.Accept:
                            yield return trueRange.Value;
                            break;

                        case RuleAction.Redirect:
                            openList.Enqueue((rule.NextWorkflow!, trueRange.Value));
                            break;
                    }
                }

                if (falseRange.HasValue)
                {
                    remainingPartRange = falseRange.Value;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private static (PartRange? trueRange, PartRange? falseRange) EvaluateRule(Rule rule, PartRange partRange)
    {
        switch (rule.Condition)
        {
            case RuleCondition.Always:
                return (partRange, null);

            case RuleCondition.LessThan:
            {
                var valueRange = partRange.GetRange(rule.Input);
                var conditionHit = valueRange.Min < rule.Value;
                if (!conditionHit)
                    return (null, partRange);

                var lessThanRange = new ValueRange(valueRange.Min, rule.Value - 1);
                var remainingRange = new ValueRange(rule.Value, valueRange.Max);

                var lessThanPartRange = partRange.WithRange(rule.Input, lessThanRange);
                var remainingPartRange = partRange.WithRange(rule.Input, remainingRange);

                return (lessThanPartRange, remainingPartRange);
            }


            case RuleCondition.MoreThan:
            {
                var valueRange = partRange.GetRange(rule.Input);
                var conditionHit = valueRange.Max > rule.Value;
                if (!conditionHit)
                    return (null, partRange);

                var moreThanRange = new ValueRange(rule.Value + 1, valueRange.Max);
                var remainingRange = new ValueRange(valueRange.Min, rule.Value);

                var moreThanPartRange = partRange.WithRange(rule.Input, moreThanRange);
                var remainingPartRange = partRange.WithRange(rule.Input, remainingRange);

                return (moreThanPartRange, remainingPartRange);
            }

            default: throw new UnreachableException();
        }
    }
}