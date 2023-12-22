namespace Utils;

public static class MathHelpers
{
	static long GetGreatestCommonDenominator(IEnumerable<long> numbers)
	{
		return numbers.Aggregate(GetGCM);
	}

	static long GetGCM(long a, long b)
	{
		return b == 0 ? a : GetGCM(b, a % b);
	}

	public static long LeastCommonMultiple(this IEnumerable<int> values)
		=> LeastCommonMultiple(values.Select(c => (long)c));

	public static long LeastCommonMultiple(this IEnumerable<long> values)
	{
		var gcm = GetGreatestCommonDenominator(values);
		return values.Aggregate((a, b) => a / gcm * b);
	}
}
