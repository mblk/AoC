namespace day08;

public static class Program
{
    private enum Operation
    {
        Increase,
        Decrease,
    }
    
    private enum Relation
    {
        Equal,
        NotEqual,
        LessThan,
        MoreThan,
        LessThanOrEqual,
        MoreThanOrEqual,
    }
    
    private record Instruction(string TargetRegister, Operation Operation, int Argument,
        string ConditionRegister, Relation ConditionRelation, int ConditionArgument);
    
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Missing input file");
            return;
        }
        
        var instructions = ParseInputFile(args[0]);
        var result = ExecuteInstructions(instructions);

        Console.WriteLine($"Max value after execution: {result.MaxValueAfterExecution}");
        Console.WriteLine($"Max value during execution: {result.MaxValueDuringExecution}");
    }

    private static (int MaxValueAfterExecution, int MaxValueDuringExecution) ExecuteInstructions(IEnumerable<Instruction> instructions)
    {
        var values = new Dictionary<string, int>();
        var maxValueDuringExecution = 0;

        int getValue(string r) => values.TryGetValue(r, out var v) ? v : 0;

        void setValue(string r, int v)
        {
            values[r] = v;
            maxValueDuringExecution = Math.Max(maxValueDuringExecution, v);
        }
        
        foreach (var instruction in instructions)
        {
            var conditionValue = getValue(instruction.ConditionRegister);
            var conditionMet = EvaluateCondition(conditionValue, instruction.ConditionRelation,
                instruction.ConditionArgument);
            if (!conditionMet) continue;

            var oldValue = getValue(instruction.TargetRegister);
            var newValue = ApplyOperation(oldValue, instruction.Operation, instruction.Argument);
            
            setValue(instruction.TargetRegister, newValue);
        }

        return (values.Values.Max(), maxValueDuringExecution);
    }

    private static bool EvaluateCondition(int value, Relation relation, int argument)
    {
        return relation switch
        {
            Relation.Equal => value == argument,
            Relation.NotEqual => value != argument,
            Relation.LessThan => value < argument,
            Relation.MoreThan => value > argument,
            Relation.LessThanOrEqual => value <= argument,
            Relation.MoreThanOrEqual => value >= argument,
            _ => throw new ArgumentOutOfRangeException(nameof(relation), relation, null)
        };
    }

    private static int ApplyOperation(int oldValue, Operation operation, int argument)
    {
        return operation switch
        {
            Operation.Increase => oldValue + argument,
            Operation.Decrease => oldValue - argument,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
    
    private static IEnumerable<Instruction> ParseInputFile(string filename)
    {
        var instructions = File.ReadLines(filename)
            .Select(ParseInstruction)
            .ToArray();

        return instructions;
    }

    private static Instruction ParseInstruction(string input)
    {
        var parts = input.Split(' ');
        if (parts.Length != 7) throw new ArgumentException($"Invalid instruction: {input}");
        if (parts[3] != "if") throw new ArgumentException($"Missing if: {input}");

        return new Instruction(
            TargetRegister: parts[0],
            Operation: ParseOperation(parts[1]),
            Argument: Int32.Parse(parts[2]),
            ConditionRegister: parts[4],
            ConditionRelation: ParseRelation(parts[5]),
            ConditionArgument: Int32.Parse(parts[6]));
    }

    private static Operation ParseOperation(string input)
    {
        return input switch
        {
            "inc" => Operation.Increase,
            "dec" => Operation.Decrease,
            _ => throw new ArgumentException($"Invalid operation: {input}"),
        };
    }

    private static Relation ParseRelation(string input)
    {
        return input switch
        {
            "==" => Relation.Equal,
            "!=" => Relation.NotEqual,
            "<" => Relation.LessThan,
            ">" => Relation.MoreThan,
            "<=" => Relation.LessThanOrEqual,
            ">=" => Relation.MoreThanOrEqual,
            _ => throw new ArgumentException($"Invalid relation: {input}"),
        };
    }
}
