namespace Day20.Modules;

internal class ConjuctionModule : Module
{
	public Dictionary<string, bool> Memory { get; }
	public bool ShouldWatchForCycles { get; private set; } = false;
	public bool ValueToWatch { get; private set; } = false;
	public int? Cycle { get; private set; }
	public bool IsCycleFound { get; private set; }

	public ConjuctionModule(string name, List<string> connectedModules) : base(name, connectedModules)
	{
		Memory = new Dictionary<string, bool>();
	}

	public override List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules)
	{
		Memory[pulse.from] = pulse.value;
		var toSend = Memory.Values.All(v => v) ? false : true;

		if (ShouldWatchForCycles == true && toSend == ValueToWatch)
		{
			if (Cycle == null)
			{
				Cycle = iteration;
			}
			else
			{
				IsCycleFound = iteration % Cycle.Value == 0;
			}
		}

		return PulseToAll(toSend);
	}

	public void WatchForCyclesOf(bool value)
	{
		ShouldWatchForCycles = true;
		ValueToWatch = value;
	}

	protected override void ReceiveConnection(string from, Dictionary<string, Module> modules)
	{
		Memory[from] = false;
	}
}
