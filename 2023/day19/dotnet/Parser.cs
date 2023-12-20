using day19.Data;

namespace day19;

public static class Parser
{
    public static (IReadOnlyList<Workflow>, IReadOnlyList<Part>) ParseWorkflowsAndParts(string filename)
    {
        var workflows = new List<Workflow>();
        var parts = new List<Part>();

        var parsingWorkflows = true;
        foreach (var line in File.ReadLines(filename))
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                parsingWorkflows = false;
                continue;
            }

            if (parsingWorkflows)
            {
                workflows.Add(ParseWorkflow(line));
            }
            else
            {
                parts.Add(ParsePart(line));
            }
        }

        return (workflows, parts);
    }

    private static Workflow ParseWorkflow(string input)
    {
        // e.g.: px{a<2006:qkq,m>2090:A,rfg}
        
        var parts = input
            .TrimEnd('}')
            .Split('{', StringSplitOptions.TrimEntries);
        if (parts.Length != 2) throw new ArgumentException("Syntax error");

        var name = parts[0];

        var rules = parts[1]
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(ParseRule)
            .ToArray();

        return new Workflow(name, rules);
    }

    private static Rule ParseRule(string input)
    {
        // e.g.:
        // a<2006:qkq
        // x>2440:R
        // A
        
        PartValue ruleInput;
        RuleCondition ruleCondition;
        int ruleValue;

        string actionString;

        // Parse optional condition.
        var colonIndex = input.IndexOf(':');
        if (colonIndex != -1)
        {
            var conditionString = input[0..colonIndex];
            actionString = input[(colonIndex + 1)..];

            ruleInput = ParseRuleInput(conditionString[0]);
            ruleCondition = ParseRuleCondition(conditionString[1]);
            ruleValue = Int32.Parse(conditionString[2..]);
        }
        else
        {
            actionString = input;

            ruleInput = PartValue.X;
            ruleCondition = RuleCondition.Always;
            ruleValue = 0;
        }

        // Parse mandatory action.
        RuleAction ruleAction;
        string? nextWorkflow = null;

        switch (actionString)
        {
            case "A":
                ruleAction = RuleAction.Accept;
                break;
            case "R":
                ruleAction = RuleAction.Reject;
                break;
            default:
                ruleAction = RuleAction.Redirect;
                nextWorkflow = actionString;
                break;
        }

        return new Rule(ruleInput, ruleCondition, ruleValue, ruleAction, nextWorkflow);
    }

    private static PartValue ParseRuleInput(char input) => input switch
    {
        'x' => PartValue.X,
        'm' => PartValue.M,
        'a' => PartValue.A,
        's' => PartValue.S,
        _ => throw new ArgumentException("Invalid rule input"),
    };

    private static RuleCondition ParseRuleCondition(char input) => input switch
    {
        '<' => RuleCondition.LessThan,
        '>' => RuleCondition.MoreThan,
        _ => throw new ArgumentException("Invalid rule condition"),
    };

    private static Part ParsePart(string input)
    {
        // e.g.: {x=787,m=2655,a=1222,s=2876}
        
        var part = new Part(0, 0, 0, 0);

        var elements = input.Trim('{', '}')
            .Split(',', StringSplitOptions.TrimEntries);

        foreach (var element in elements)
        {
            if (element.Split('=', StringSplitOptions.TrimEntries)
                    is not [var key, var v] ||
                !Int32.TryParse(v, out var value))
                throw new ArgumentException("Syntax error");

            part = key switch
            {
                "x" => part with { X = value },
                "m" => part with { M = value },
                "a" => part with { A = value },
                "s" => part with { S = value },
                _ => throw new ArgumentException("Invalid element key"),
            };
        }

        return part;
    }
}