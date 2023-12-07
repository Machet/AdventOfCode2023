var hands = File.ReadAllLines("input.txt")
	.Select(x => x.Split(" "))
	.Select(x => new Hand(x[0], long.Parse(x[1])))
	.ToList();

var result1 = hands
	.OrderBy(x => x, new HandComparer())
	.Select((h, i) => h.Bid * (i + 1))
	.Sum();

var result2 = hands
	.Select(h => new JockerHand(h.Value, h.Bid))
	.OrderBy(x => x, new JockerHandComparer())
	.Select((h, i) => h.Bid * (i + 1))
	.Sum();

Console.WriteLine("1 : " + result1);
Console.WriteLine("2 : " + result2);
// 1 : 248217452
// 2 : 245576185



public record Hand(string Value, long Bid)
{
	public long Strength = Value.GroupBy(Value => Value).Select(g => g.Count() == 1 ? 1 : (long)Math.Pow(2, g.Count())).Sum();
	public List<Card> Cards = Value.Select(v => new Card(v)).ToList();
}

public record JockerHand(string Value, long Bid)
{
	public long Strength = GetTokens(Value).GroupBy(Value => Value).Select(g => g.Count() == 1 ? 1 : (long)Math.Pow(2, g.Count())).Sum();
	public List<Card> Cards = Value.Select(v => new Card(v)).ToList();

	private static IEnumerable<char> GetTokens(string value)
	{
		var best = value.Where(x => x != 'J').GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).DefaultIfEmpty('J').FirstOrDefault();
		foreach (var c in value)
		{
			yield return c == 'J' ? best : c;
		}
	}
}

public record Card(char token)
{
	public long Strength = GetStrength(token);
	public long Strength2 = token == 'J' ? 1 : GetStrength(token);

	private static long GetStrength(char token)
	{
		return token switch
		{
			'A' => 14,
			'K' => 13,
			'Q' => 12,
			'J' => 11,
			'T' => 10,
			'9' => 9,
			'8' => 8,
			'7' => 7,
			'6' => 6,
			'5' => 5,
			'4' => 4,
			'3' => 3,
			'2' => 2,
			_ => throw new ArgumentException()
		};
	}
}

public class HandComparer : IComparer<Hand>
{
	public int Compare(Hand? x, Hand? y)
	{
		var handStrength = x!.Strength.CompareTo(y!.Strength);

		if (handStrength != 0) return handStrength;

		for (int i = 0; i < x?.Cards.Count; i++)
		{
			var cardStrength = x.Cards[i].Strength.CompareTo(y!.Cards[i].Strength);
			if (cardStrength != 0) return cardStrength;
		}

		return 0;
	}
}

public class JockerHandComparer : IComparer<JockerHand>
{
	public int Compare(JockerHand? x, JockerHand? y)
	{
		var handStrength = x!.Strength.CompareTo(y!.Strength);

		if (handStrength != 0) return handStrength;

		for (int i = 0; i < x?.Cards.Count; i++)
		{
			var cardStrength = x.Cards[i].Strength2.CompareTo(y!.Cards[i].Strength2);
			if (cardStrength != 0) return cardStrength;
		}

		return 0;
	}
}