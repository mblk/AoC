namespace day20;

public class PulseSimulator
{
    private class ModuleState
    {
        public required bool FlipFlopState;
        public required Dictionary<string, bool> PrevInputs;
    }
    
    private readonly IReadOnlyDictionary<string, Module> _modules;
    private readonly IReadOnlyDictionary<string, ModuleState> _moduleStates;

    public PulseSimulator(IReadOnlyCollection<Module> modules)
    {
        _modules = modules.ToDictionary(m => m.Name, m => m);

        _moduleStates = modules.ToDictionary(m => m.Name, m => new ModuleState
        {
            FlipFlopState = false,
            PrevInputs = m.InputModules.ToDictionary(name => name, _ => false),
        });
    }

    public (int, int) SimulateAndCountPulses(int numButtonPresses)
    {
        var totalNumLow = 0;
        var totalNumHigh = 0;

        for (var i = 1; i <= numButtonPresses; i++)
        {
            var (numLow, numHigh, _) = Simulate();
            totalNumLow += numLow;
            totalNumHigh += numHigh;
        }

        return (totalNumLow, totalNumHigh);
    }

    public int SimulateUntilHighPulse(string targetModuleName)
    {
        for (var i = 1;; i++)
        {
            var (_, _, wasHigh) = Simulate(targetModuleName);
            if (wasHigh)
                return i;
        }
    }

    private (int, int, bool) Simulate(string? highPulseTargetModule = null)
    {
        var numLowPulses = 0;
        var numHighPulses = 0;

        var pulseEther = new Queue<(string, bool, string)>();
        pulseEther.Enqueue(("button", false, "broadcaster"));

        void send(Module sender, bool pulse)
        {
            foreach (var output in sender.OutputModules)
            {
                pulseEther.Enqueue((sender.Name, pulse, output));
            }
        }

        while (pulseEther.Count > 0)
        {
            var (sender, pulse, receiver) = pulseEther.Dequeue();

            var module = _modules[receiver];
            var moduleState = _moduleStates[receiver];

            if (pulse == false)
                numLowPulses++;
            else
                numHighPulses++;

            if (sender == highPulseTargetModule && pulse)
            {
                return (0, 0, true);
            }

            switch (module.Type)
            {
                case ModuleType.Broadcaster:
                {
                    send(module, pulse);
                    break;
                }

                case ModuleType.FlipFlop:
                {
                    if (pulse == false)
                    {
                        moduleState.FlipFlopState ^= true;
                        send(module, moduleState.FlipFlopState);
                    }
                    break;
                }

                case ModuleType.Conjunction:
                {
                    moduleState.PrevInputs[sender] = pulse;

                    var allInputsHigh = moduleState.PrevInputs.Values
                        .All(s => s);

                    send(module, !allInputsHigh);
                    break;
                }
            }
        }

        return (numLowPulses, numHighPulses, false);
    }
}