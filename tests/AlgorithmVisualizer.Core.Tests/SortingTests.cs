using AlgorithmVisualizer.Core.Inputs;
using AlgorithmVisualizer.Core.Sorting;

namespace AlgorithmVisualizer.Core.Tests;

public class SortingTests
{
    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Sorts_empty_and_single_element_arrays(string id)
    {
        var algorithm = TestData.Sort(id);

        int[] empty = [];
        algorithm.Sort(empty);
        Assert.Empty(empty);

        int[] single = [42];
        algorithm.Sort(single);
        Assert.Equal(42, Assert.Single(single));
    }

    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Sorts_random_arrays_exactly_like_Array_Sort(string id)
    {
        var algorithm = TestData.Sort(id);
        var random = new Random(20261117);
        int[] lengths = [2, 3, 5, 16, 31, 64, 100, 257, 1000];
        foreach (int length in lengths)
        {
            for (int trial = 0; trial < 5; trial++)
            {
                int[] values = TestData.RandomInts(random, length, -1_000, 1_000);
                int[] expected = [.. values.Order()];

                algorithm.Sort(values);

                Assert.Equal(expected, values);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Sorts_every_input_shape_the_visualizer_offers(string id)
    {
        var algorithm = TestData.Sort(id);
        int[] lengths = [0, 1, 2, 7, 50, 200];
        foreach (var shape in Enum.GetValues<InputShape>())
        {
            foreach (int length in lengths)
            {
                int[] values = InputGenerator.Create(shape, length, seed: length);
                int[] expected = [.. values.Order()];

                algorithm.Sort(values);

                Assert.Equal(expected, values);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.FullRangeSorts), MemberType = typeof(TestData))]
    public void Handles_negative_numbers_and_the_int_extremes(string id)
    {
        var algorithm = TestData.Sort(id);

        int[] edges = [0, int.MaxValue, -1, int.MinValue, 7, int.MinValue, int.MaxValue, -7, 1];
        int[] expectedEdges = [.. edges.Order()];
        algorithm.Sort(edges);
        Assert.Equal(expectedEdges, edges);

        int[] wide = TestData.RandomInts(new Random(99), 500, int.MinValue, int.MaxValue);
        int[] expectedWide = [.. wide.Order()];
        algorithm.Sort(wide);
        Assert.Equal(expectedWide, wide);
    }

    [Theory]
    [MemberData(nameof(TestData.ComparisonSorts), MemberType = typeof(TestData))]
    public void Comparison_sorts_follow_the_comparer_they_are_given(string id)
    {
        var descending = Comparer<int>.Create((x, y) => y.CompareTo(x));
        int[] values = TestData.RandomInts(new Random(5), 200, 0, 50);
        int[] expected = [.. values.OrderDescending()];

        TestData.Sort(id).Sort(values, descending);

        Assert.Equal(expected, values);
    }

    [Theory]
    [MemberData(nameof(TestData.IntegerSorts), MemberType = typeof(TestData))]
    public void Integer_sorts_make_no_comparisons_at_all(string id)
    {
        int[] values = TestData.RandomInts(new Random(8), 300, -500, 500);
        int[] expected = [.. values.Order()];

        var counts = TestData.Sort(id).Sort(values);

        Assert.Equal(expected, values);
        Assert.Equal(0, counts.Comparisons);
    }

    [Theory]
    [MemberData(nameof(TestData.IntegerSorts), MemberType = typeof(TestData))]
    public void Integer_sorts_explain_that_they_need_an_integer_key(string id)
    {
        string[] words = ["pear", "fig", "apple"];

        var error = Assert.Throws<InvalidOperationException>(() => TestData.Sort(id).Sort(words));

        Assert.Contains("integer key", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Integer_sorts_order_records_by_the_key_they_are_given()
    {
        (string Name, int Age)[] people = [("Ana", 31), ("Ben", 25), ("Cy", 31), ("Di", 19)];

        new RadixSort().Sort(people, integerKey: p => p.Age);

        string[] expected = ["Di", "Ben", "Ana", "Cy"];
        Assert.Equal(expected, people.Select(p => p.Name));
    }

    [Fact]
    public void Counting_sort_refuses_a_key_range_it_cannot_count_and_points_to_radix_sort()
    {
        int[] values = [int.MinValue, 0, int.MaxValue];

        var error = Assert.Throws<ArgumentOutOfRangeException>(() => new CountingSort().Sort(values));

        Assert.Contains("radix sort", error.Message, StringComparison.Ordinal);
    }
}
