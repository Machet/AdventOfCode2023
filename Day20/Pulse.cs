namespace Day20;

record Pulse(string from, bool value, string destination)
{
	public override string ToString()
	{
		return $"{from} ({(value ? "high" : "low")}) -> {destination}";
	}
}
