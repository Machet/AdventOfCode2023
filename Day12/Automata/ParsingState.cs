namespace Day12.Automata;

abstract class ParsingState
{
	public string Name { get; }
	public int ExpectedLength { get; }

	public ParsingState(string name, int expectedLength)
	{
		Name = name;
		ExpectedLength = expectedLength;
	}

	public List<ParsingState> GetNextStates(char letter, int remainingLength)
	{
		if (ExpectedLength > remainingLength) return new List<ParsingState>();

		return GoToNextStates(letter, remainingLength);
	}

	public virtual bool IsFinished => false;
	protected abstract List<ParsingState> GoToNextStates(char letter, int remainingLength);

	public virtual void Print()
	{
		Console.WriteLine($"{Name} : {ExpectedLength}");
	}

	public override string ToString()
	{
		return Name;
	}
}
