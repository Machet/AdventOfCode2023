namespace Day12.Automata;

internal class WaitForEnd : ParsingState
{
	public WaitForEnd(string name)
		: base(name, 0)
	{
	}

	public override bool IsFinished => true;

	protected override List<ParsingState> GoToNextStates(char letter, int remainingLength)
	{
		return letter switch
		{
			'.' => new() { this },
			'?' => new() { this },
			'#' => new() { },
			_ => throw new InvalidOperationException()
		};
	}
}
