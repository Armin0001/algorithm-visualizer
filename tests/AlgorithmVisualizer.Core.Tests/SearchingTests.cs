using AlgorithmVisualizer.Core.Searching;

namespace AlgorithmVisualizer.Core.Tests;

public class SearchingTests
{
    [Theory]
    [MemberData(nameof(TestData.AllSearches), MemberType = typeof(TestData))]
    public void Find_the_first_match_exactly_like_Array_IndexOf(string id)
    {
        var search = TestData.Search(id);
        var random = new Random(41);
        for (int trial = 0; trial < 500; trial++)
        {
            int[] values = [.. TestData.RandomInts(random, random.Next(0, 100), 0, 40).Order()];
            int target = random.Next(-3, 44); // below, inside and above the range

            Assert.Equal(Array.IndexOf(values, target), search.Search(values, target));
        }
    }

    [Theory]
    [MemberData(nameof(TestData.AllSearches), MemberType = typeof(TestData))]
    public void Return_minus_one_when_there_is_nothing_to_find(string id)
    {
        var search = TestData.Search(id);

        Assert.Equal(-1, search.Search([], 5));
        Assert.Equal(-1, search.Search([1, 3, 5], 0)); // before the first element
        Assert.Equal(-1, search.Search([1, 3, 5], 4)); // in a gap
        Assert.Equal(-1, search.Search([1, 3, 5], 9)); // after the last element
    }

    [Theory]
    [MemberData(nameof(TestData.AllSearches), MemberType = typeof(TestData))]
    public void Find_the_first_of_several_equal_values(string id)
    {
        int[] values = [1, 2, 2, 2, 2, 2, 3];

        Assert.Equal(1, TestData.Search(id).Search(values, 2));
    }

    [Fact]
    public void Linear_search_does_not_need_sorted_input()
    {
        int[] values = [9, 4, 7, 4, 1];

        Assert.Equal(2, new LinearSearch().Search(values, 7));
        Assert.Equal(1, new LinearSearch().Search(values, 4));
    }

    [Fact]
    public void Linear_search_checks_every_element_when_the_target_is_missing()
    {
        var tracked = new TrackedArray<int>([.. Enumerable.Range(0, 250)]);

        Assert.Equal(-1, new LinearSearch().Search(tracked, 1000));
        Assert.Equal(250, tracked.Counts.Comparisons);
    }

    [Fact]
    public void Binary_search_makes_at_most_floor_log2_n_plus_2_comparisons()
    {
        foreach (int n in Lengths)
        {
            int[] evens = [.. Enumerable.Range(0, n).Select(i => i * 2)];
            long budget = (long)Math.Floor(Math.Log2(n)) + 2;
            foreach (int target in Targets(evens))
            {
                var tracked = new TrackedArray<int>(evens);

                new BinarySearch().Search(tracked, target);

                Assert.InRange(tracked.Counts.Comparisons, 1, budget);
            }
        }
    }

    [Fact]
    public void Jump_search_makes_about_2_sqrt_n_comparisons()
    {
        foreach (int n in Lengths)
        {
            int[] evens = [.. Enumerable.Range(0, n).Select(i => i * 2)];
            long budget = (2 * (long)Math.Ceiling(Math.Sqrt(n))) + 2;
            foreach (int target in Targets(evens))
            {
                var tracked = new TrackedArray<int>(evens);

                new JumpSearch().Search(tracked, target);

                Assert.InRange(tracked.Counts.Comparisons, 1, budget);
            }
        }
    }

    private static int[] Lengths => [1, 2, 3, 7, 8, 9, 100, 1000, 1 << 16];

    // Present at the start, middle and end, plus missing below, above and in a gap (the values are even).
    private static int[] Targets(int[] evens) => [-1, evens[0], evens[evens.Length / 2], evens[^1], evens[^1] + 1, 1];
}
