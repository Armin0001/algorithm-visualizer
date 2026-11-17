namespace AlgorithmVisualizer.Core.Tests;

/// <summary>
/// The UI labels every sort stable or unstable, so the label is tested in both directions: stable sorts
/// must never reorder equal keys, and unstable ones must be caught doing it at least once.
/// </summary>
public class StabilityTests
{
    private static readonly IComparer<Card> ByRank = Comparer<Card>.Create((a, b) => a.Rank.CompareTo(b.Rank));

    [Theory]
    [MemberData(nameof(TestData.StableSorts), MemberType = typeof(TestData))]
    public void Stable_sorts_keep_equal_keys_in_their_original_order(string id)
    {
        var algorithm = TestData.Sort(id);
        var random = new Random(31);
        for (int trial = 0; trial < 100; trial++)
        {
            Card[] cards = Deal(random, random.Next(2, 80), ranks: 6);

            algorithm.Sort(cards, ByRank, c => c.Rank);

            for (int i = 1; i < cards.Length; i++)
            {
                Assert.True(cards[i - 1].Rank <= cards[i].Rank, $"{id} left the cards out of rank order.");
                if (cards[i - 1].Rank == cards[i].Rank)
                {
                    Assert.True(
                        cards[i - 1].DealtAt < cards[i].DealtAt,
                        $"{id} moved the card dealt at {cards[i].DealtAt} ahead of an equal card dealt at {cards[i - 1].DealtAt}.");
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.UnstableSorts), MemberType = typeof(TestData))]
    public void Sorts_marked_unstable_really_do_reorder_equal_keys(string id)
    {
        var algorithm = TestData.Sort(id);
        var random = new Random(32);
        bool reordered = false;
        for (int trial = 0; trial < 500 && !reordered; trial++)
        {
            Card[] cards = Deal(random, random.Next(2, 40), ranks: 3);

            algorithm.Sort(cards, ByRank, c => c.Rank);

            reordered = cards.Zip(cards.Skip(1)).Any(pair => pair.First.Rank == pair.Second.Rank && pair.First.DealtAt > pair.Second.DealtAt);
        }

        Assert.True(reordered, $"{id} is marked unstable, but it kept equal keys in order on every input tried.");
    }

    private static Card[] Deal(Random random, int length, int ranks) =>
        [.. Enumerable.Range(0, length).Select(i => new Card(random.Next(ranks), i))];

    /// <summary>A card's rank is the sort key; the order it was dealt in reveals whether equal ranks moved.</summary>
    private sealed record Card(int Rank, int DealtAt);
}
