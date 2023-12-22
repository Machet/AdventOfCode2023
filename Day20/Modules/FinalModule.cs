namespace Day20.Modules;

internal class FinalModule : Module
{
	public int StartingPulses { get; private set; }

	public FinalModule(string name) : base(name, new List<string>())
	{
	}

	public override List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules)
	{
		if (pulse.value == false)
		{
			StartingPulses++;
		}

		return new List<Pulse>();
	}

	public void Reset()
	{
		StartingPulses = 0;
	}
}
