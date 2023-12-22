namespace Day20.Modules;
internal class TestModule : Module
{
	public TestModule(string name) : base(name, new List<string>())
	{
	}

	public override List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules)
	{
		return new List<Pulse>();
	}
}
