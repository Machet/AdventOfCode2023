namespace Day20.Modules;

internal class FlipFlopModule : Module
{
    public bool IsOn { get; private set; }

    public FlipFlopModule(string name, List<string> connectedModules) : base(name, connectedModules)
    {
        IsOn = false;
    }

    public override List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules)
    {
        if (pulse.value)
        {
            return new List<Pulse>();
        }

        if (IsOn)
        {
            IsOn = false;
            return PulseToAll(false);
        }

        IsOn = true;
        return PulseToAll(true);
    }
}
