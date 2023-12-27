namespace Day12.Automata;

internal class WaitForGroup : ParsingState
{
	private readonly ParsingState _nextState;

	public WaitForGroup(string name, ParsingState next, int expectedLength)
			: base(name, expectedLength)
	{
		_nextState = next;
	}

	public override void Print()
	{
		base.Print();
		_nextState.Print();
	}

	protected override List<ParsingState> GoToNextStates(char letter, int remainingLength)
	{
		return letter switch
		{
			'.' => new() { this },
			'#' => new() { _nextState },
			'?' => new() { this, _nextState },
			_ => throw new InvalidOperationException()
		};
	}
}
