using Utils;

var input = File.ReadAllLines("input.txt")
	.Select(line => Parse(line))
	.ToList();

var unfoldedInput = input.Select(i => new DataRecord(
		string.Join("?", Enumerable.Repeat(i.template, 5)),
		Enumerable.Repeat(i.groups, 5).SelectMany(x => x).ToList()))
	.ToList();

long count = 0;
for (int i = 0; i < unfoldedInput.Count; i++)
{
	count += GetPossibleTemplatesCount(unfoldedInput[i]);
	Console.WriteLine(count);
}

Console.WriteLine("1: " + input.Select(i => GetPossibleTemplatesCount(i)).Sum());
Console.WriteLine("2: " + count);

long GetPossibleTemplatesCount(DataRecord record)
{
	var toProcess = new Stack<State>();
	toProcess.Push(new State(record.template, record.groups, 0));
	long count = 0;

	while (toProcess.Count > 0)
	{
		var state = toProcess.Pop();
		foreach (var nextState in GetNextStates(state).ToList())
		{
			if (nextState.IsComplete)
			{
				if (nextState.groups.Count == 0)
				{
					count++;
				}
			}
			else
			{
				toProcess.Push(nextState);
			}
		}
	}

	return count;
}

IEnumerable<State> GetNextStates(State state)
{
	if (!state.CanFitGroups) yield break;

	var nextGroup = state.groups.Select(i => (int?)i).FirstOrDefault();

	if (nextGroup != null && CanFitGroup(state.template, state.startingPosition, nextGroup.Value))
	{
		yield return new State(state.template, state.groups.Skip(1).ToList(), state.startingPosition + nextGroup.Value + 1);
	}

	if (CanFitSpace(state.template, state.startingPosition))
	{
		yield return new State(state.template, state.groups, state.startingPosition + 1);
	}
}

bool CanFitGroup(string template, int pos, int length)
{
	if (pos + length > template.Length) return false;

	for (int i = pos; i < pos + length; i++)
	{
		if (template[i] == '.') return false;
	}

	return CanFitSpace(template, pos + length);
}

string ReplaceWithGroup(string template, int startingPosition, int value)
{
	var x = template.Remove(startingPosition, value);
	x = x.Insert(startingPosition, new string(Enumerable.Repeat('#', value).ToArray()));
	return ReplaceWithSpace(x, startingPosition + value);
}

bool CanFitSpace(string template, int pos)
{
	return template.Length <= pos || template[pos] != '#';
}

string ReplaceWithSpace(string template, int startingPosition)
{
	if (startingPosition >= template.Length) return template;

	var x = template.Remove(startingPosition, 1);
	return x.Insert(startingPosition, ".");
}

DataRecord Parse(string line)
{
	var x = line.Split(" ");
	return new DataRecord(x[0], x[1].ToIntList(','));
}

record DataRecord(string template, List<int> groups);
record State(string template, List<int> groups, int startingPosition)
{
	public int GroupsLength = groups.Any() ? groups.Sum() + groups.Count() - 1 + startingPosition : 0;
	public bool CanFitGroups = !groups.Any() || groups.Sum() + groups.Count() - 1 + startingPosition <= template.Length;
	public bool IsComplete = startingPosition >= template.Length;

	public override string ToString()
	{
		return $"{(!IsComplete ? template.Insert(startingPosition, "|") : template)} {string.Join(",", groups)}, pos: {startingPosition})";
	}
}