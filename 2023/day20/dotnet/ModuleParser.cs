namespace day20;

public static class ModuleParser
{
    public static Module[] ParseModules(string filename)
    {
        var modules = File.ReadLines(filename)
            .Select(ParseModule)
            .ToArray();

        var additionalModules = modules
            .SelectMany(m => m.OutputModules)
            .Distinct()
            .Except(modules.Select(m => m.Name))
            .ToArray();

        if (additionalModules.Any())
        {
            modules = modules
                .Concat(additionalModules.Select(CreateDummyModule))
                .ToArray();
        }
        
        var inputs = modules.ToDictionary(m => m.Name, _ => new List<string>());

        foreach (var module in modules)
        {
            foreach (var output in module.OutputModules)
            {
                inputs[output].Add(module.Name);
            }
        }

        foreach (var module in modules)
        {
            module.InputModules = inputs[module.Name].ToArray();
        }

        return modules;
    }

    private static Module ParseModule(string input)
    {
        var type = ModuleType.Broadcaster;
        if (input.StartsWith('%'))
        {
            type = ModuleType.FlipFlop;
            input = input[1..];
        }
        else if (input.StartsWith('&'))
        {
            type = ModuleType.Conjunction;
            input = input[1..];
        }

        var parts = input.Split("->", StringSplitOptions.TrimEntries);
        var name = parts[0];
        var outputs = parts[1].Split(',', StringSplitOptions.TrimEntries);

        return new Module
        {
            Name = name,
            Type = type,
            OutputModules = outputs,
            InputModules = Array.Empty<string>(),
        };
    }

    private static Module CreateDummyModule(string name)
    {
        return new Module
        {
            Name = name,
            Type = ModuleType.Dummy,
            OutputModules = Array.Empty<string>(),
            InputModules = Array.Empty<string>(),
        };
    }
}