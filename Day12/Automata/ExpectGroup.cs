namespace Day12.Automata;

internal class ExpectGroup : ParsingState
{
	private readonly ParsingState _nextState;

	public ExpectGroup(string name, ParsingState next, int expectedLength)
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
			'.' => new() { },
			'?' => new() { _nextState },
			'#' => new() { _nextState },
			_ => throw new InvalidOperationException()
		};
	}
}
