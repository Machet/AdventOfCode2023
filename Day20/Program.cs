using System.Data;
using Day20;
using Day20.Modules;
using Utils;

var modules = GetModules();

var result1 = PressButton(modules, 1000);
Console.WriteLine("1: " + result1);

var modules2 = GetModules();
var outputConjuction = GetConjuctionModulesConnectedTo("rx", modules2);
var feedingConjuctions = GetConjuctionModulesConnectedTo(outputConjuction.First().Name, modules2);

foreach (var module in feedingConjuctions)
{
	module.WatchForCyclesOf(true);
}

var i = 1;
while (!feedingConjuctions.All(c => c.IsCycleFound))
{
	ProcessAndCount(i++, modules2);
}
// 577520933 too low
var cycleValues = feedingConjuctions.Select(c => c.Cycle!.Value).ToList();
var leastCommonMultiple = MathHelpers.LeastCommonMultiple(cycleValues);

Console.WriteLine("2: " + leastCommonMultiple);

static long PressButton(Dictionary<string, Module> modules, int times)
{
	long high = 0;
	long low = 0;

	for (int i = 0; i < times; i++)
	{
		var result = ProcessAndCount(i, modules);
		high += result.high;
		low += result.low;
	}

	return high * low;
}

static (int low, int high) ProcessAndCount(int iteration, Dictionary<string, Module> modules)
{
	var toProcess = new Queue<Pulse>();
	toProcess.Enqueue(new Pulse("button", false, "broadcaster"));
	var lows = 0;
	var highs = 0;

	while (toProcess.Count > 0)
	{
		var pulse = toProcess.Dequeue();
		lows += pulse.value ? 0 : 1;
		highs += pulse.value ? 1 : 0;
		var dest = modules[pulse.destination];

		var newPulses = dest.ProcessPulse(iteration, pulse, modules);

		foreach (var p in newPulses)
		{
			toProcess.Enqueue(p);
		}
	}

	return (lows, highs);
}

List<ConjuctionModule> GetConjuctionModulesConnectedTo(string output, Dictionary<string, Module> modules)
{
	return modules.Values.Where(m => m.ConnectedModules.Contains(output)).Cast<ConjuctionModule>().ToList();
}

Dictionary<string, Module> GetModules()
{
	var modules = File.ReadAllLines("input.txt")
		.Select(line => ParseModule(line))
		.ToDictionary(m => m.Name, m => m);

	modules.Add("output", new TestModule("output"));
	modules.Add("rx", new FinalModule("rx"));

	foreach (var module in modules)
	{
		module.Value.RegisterConnections(modules);
	}

	return modules;
}

Module ParseModule(string line)
{
	var parts = line.Split(" -> ");
	var connections = parts[1].Split(",", StringSplitOptions.TrimEntries).ToList();

	if (parts[0] == "broadcaster")
	{
		return new BroadcasterModule(parts[0], connections);
	}

	if (parts[0].StartsWith("&"))
	{
		return new ConjuctionModule(parts[0].Substring(1), connections);
	}

	if (parts[0].StartsWith("%"))
	{
		return new FlipFlopModule(parts[0].Substring(1), connections);
	}

	throw new Exception();
}