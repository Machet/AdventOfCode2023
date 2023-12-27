namespace Day12.Automata;

internal class ExpectEmpty : ParsingState
{
	private readonly ParsingState _nextState;
	private readonly bool _isOptional;

	public ExpectEmpty(string name, ParsingState next, int expectedLength, bool isOptional)
		: base(name, expectedLength)
	{
		_nextState = next;
		_isOptional = isOptional;
	}

	public override bool IsFinished => _isOptional;

	public override void Print()
	{
		base.Print();
		_nextState.Print();
	}

	protected override List<ParsingState> GoToNextStates(char letter, int remainingLength)
	{
		return letter switch
		{
			'.' => new() { _nextState },
			'?' => new() { _nextState },
			'#' => new() { },
			_ => throw new InvalidOperationException()
		};
	}
}
