namespace Day20.Modules;

internal class BroadcasterModule : Module
{
    public BroadcasterModule(string name, List<string> connectedModules)
        : base(name, connectedModules)
    {
    }

    public override List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules)
    {
        return PulseToAll(pulse.value);
    }
}
