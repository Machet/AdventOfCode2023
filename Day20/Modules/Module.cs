namespace Day20.Modules;

internal abstract class Module
{
	public string Name { get; }
	public List<string> ConnectedModules { get; }

	public Module(string name, List<string> connectedModules)
	{
		Name = name;
		ConnectedModules = connectedModules;
	}

	public virtual void RegisterConnections(Dictionary<string, Module> modules)
	{
		foreach (var connected in ConnectedModules)
		{
			var module = modules[connected];
			module.ReceiveConnection(Name, modules);
		}
	}

	protected virtual void ReceiveConnection(string from, Dictionary<string, Module> modules)
	{
	}

	public abstract List<Pulse> ProcessPulse(int iteration, Pulse pulse, Dictionary<string, Module> modules);

	protected List<Pulse> PulseToAll(bool pulse)
	{
		return ConnectedModules
			.Select(dest => new Pulse(Name, pulse, dest))
			.ToList();
	}
}
