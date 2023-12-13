using Utils;

var input = File.ReadAllLines("input.txt");
var mirrors = SplitMirrors(input).ToList();

var result1 = mirrors.Select(m => GetReflectionScore(m, withSmudge: false)).ToList();
var result2 = mirrors.Select(m => GetReflectionScore(m, withSmudge: true)).ToList();

Console.WriteLine("1: " + result1.Sum());
Console.WriteLine("2: " + result2.Sum());

int GetReflectionScore(List<string> mirror, bool withSmudge)
{
	var rotated = Rotate(mirror).ToList();

	var yReflection = FindReflection(mirror, withSmudge);
	var xReflection = yReflection != 0 ? 0 : FindReflection(rotated, withSmudge);

	return xReflection + 100 * yReflection;
}

int FindReflection(List<string> mirror, bool withSmudge)
{
	var possibleMirrors = new List<int>();

	for (int i = 1; i < mirror.Count; i++)
	{
		var rowReflection = IsReflectingRow(mirror[i], mirror[i - 1], withSmudge);
		if (rowReflection.isReflecting && IsReflecting(mirror, i - 1, withSmudge))
		{
			return i;
		}
	}

	return 0;
}

bool IsReflecting(List<string> mirror, int pos, bool mustHaveSmudge)
{
	var reflectionLength = Math.Min(pos, mirror.Count - pos - 2);
	var mayHaveSmudge = mustHaveSmudge;
	for (int i = 0; i <= reflectionLength; i++)
	{
		var rowReflection = IsReflectingRow(mirror[pos - i], mirror[pos + i + 1], mayHaveSmudge);
		if (!rowReflection.isReflecting)
		{
			return false;
		}

		mayHaveSmudge = rowReflection.mayHaveSmudge;
	}

	return mustHaveSmudge
		? true && !mayHaveSmudge
		: true;
}

(bool isReflecting, bool mayHaveSmudge) IsReflectingRow(string r1, string r2, bool withSmudge)
{
	if (r1 == r2) { return (true, withSmudge); }
	if (withSmudge == false) return (false, withSmudge);

	for (int i = 0; i < r1.Length; i++)
	{
		if (r1[i] != r2[i])
		{
			if (!withSmudge)
			{
				return (false, withSmudge);
			}

			withSmudge = false;
		}
	}

	return (true, withSmudge);
}

IEnumerable<string> Rotate(List<string> mirror)
{
	var chars = mirror.ToArray().ToCharArray();
	for (int i = 0; i < chars.GetLength(1); i++)
	{
		yield return new string(chars.GetColumn(i).ToArray());
	}
}

IEnumerable<List<string>> SplitMirrors(string[] inputs)
{
	var mirror = new List<string>();

	foreach (var input in inputs)
	{
		if (string.IsNullOrEmpty(input))
		{
			yield return mirror;
			mirror = new List<string>();
		}
		else
		{
			mirror.Add(input);
		}
	}

	yield return mirror;
}