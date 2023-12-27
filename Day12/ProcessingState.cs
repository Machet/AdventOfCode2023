using Day12.Automata;

namespace Day12;

internal class ProcessingState
{
	private Dictionary<ParsingState, long> _states = new();

	public ProcessingState() { }

	public void Add(ParsingState parsingState, long count = 1)
	{
		if (!_states.ContainsKey(parsingState))
		{
			_states.Add(parsingState, 0);
		}

		_states[parsingState] += count;
	}

	public void AddRange(IEnumerable<ParsingState> parsingStates, long count = 1)
	{
		foreach (var parsingState in parsingStates)
		{
			Add(parsingState, count);
		}
	}

	internal long GetValidStatesCount()
	{
		return _states.Where(s => s.Key.IsFinished)
			.Select(s => s.Value)
			.Sum();
	}

	internal ProcessingState Process(char c, int remainingCount)
	{
		var newState = new ProcessingState();

		foreach (var state in _states)
		{
			var newStates = state.Key.GetNextStates(c, remainingCount);
			newState.AddRange(newStates, state.Value);
		}

		return newState;
	}
}
