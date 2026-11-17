using AlgorithmVisualizer.Core.Sorting;

namespace AlgorithmVisualizer.Core.Tests;

/// <summary>
/// The complexity table in the UI is a claim, so these tests check the operation counts behind it. They
/// would catch, say, a merge sort that quietly went quadratic or a bubble sort that lost its early exit.
/// </summary>
public class ComplexityTests
{
    [Theory]
    [InlineData("bubble-sort")]
    [InlineData("insertion-sort")]
    public void Adaptive_sorts_finish_sorted_input_in_a_single_pass(string id)
    {
        int[] values = [.. Enumerable.Range(0, 500)];

        var counts = TestData.Sort(id).Sort(values);

        Assert.Equal(499, counts.Comparisons);
        Assert.Equal(0, counts.Swaps);
    }

    [Theory]
    [InlineData("bubble-sort")]
    [InlineData("insertion-sort")]
    public void Adjacent_swap_sorts_swap_exactly_once_per_inversion(string id)
    {
        var random = new Random(17);
        for (int trial = 0; trial < 25; trial++)
        {
            int[] values = TestData.RandomInts(random, random.Next(0, 150), 0, 40);
            long inversions = TestData.CountInversions(values);

            var counts = TestData.Sort(id).Sort(values);

            Assert.Equal(inversions, counts.Swaps);
        }
    }

    [Fact]
    public void Selection_sort_always_makes_n_squared_over_2_comparisons_but_at_most_n_minus_1_swaps()
    {
        const int n = 300;
        foreach (int[] input in Shapes(n))
        {
            var counts = new SelectionSort().Sort(input);

            Assert.Equal((long)n * (n - 1) / 2, counts.Comparisons);
            Assert.InRange(counts.Swaps, 0, n - 1);
        }
    }

    [Fact]
    public void Merge_sort_skips_merging_halves_that_are_already_in_order()
    {
        int[] values = [.. Enumerable.Range(0, 1024)];

        var counts = new MergeSort().Sort(values);

        Assert.Equal(1023, counts.Comparisons);
        Assert.Equal(0, counts.Writes);
    }

    [Fact]
    public void Quicksort_handles_all_equal_keys_in_linear_time()
    {
        int[] values = [.. Enumerable.Repeat(7, 10_000)];

        var counts = new QuickSort().Sort(values);

        // One three-way partition pass, plus the three comparisons that pick the pivot.
        Assert.Equal(values.Length + 2, counts.Comparisons);
    }

    [Theory]
    [InlineData("merge-sort")]
    [InlineData("heap-sort")]
    [InlineData("quick-sort")]
    public void N_log_n_sorts_stay_within_their_comparison_budget(string id)
    {
        const int n = 4096;
        double log2N = Math.Log2(n);

        // Merge sort: the classic n⌈log2 n⌉ bound (the "already in order" check fits inside it).
        // Heap sort: at most 2n log2 n. Quicksort measures 0.9 to 1.2n log2 n on these shapes; this test is
        // what caught the end-sampled median of three climbing past 4n log2 n on sorted input.
        long budget = id switch
        {
            "merge-sort" => (long)(n * Math.Ceiling(log2N)),
            "quick-sort" => (long)(1.5 * n * log2N),
            _ => (long)(2 * n * log2N),
        };

        foreach (int[] input in Shapes(n))
        {
            var counts = TestData.Sort(id).Sort(input);

            Assert.InRange(counts.Comparisons, 0, budget);
        }
    }

    [Fact]
    public void Shell_sort_beats_insertion_sort_on_reversed_input()
    {
        int[] reversed = [.. Enumerable.Range(0, 2000).Reverse()];

        var shell = new ShellSort().Sort([.. reversed]);
        var insertion = new InsertionSort().Sort([.. reversed]);

        Assert.True(shell.Comparisons * 20 < insertion.Comparisons, $"Shell sort used {shell.Comparisons:N0} comparisons, insertion sort {insertion.Comparisons:N0}.");
    }

    [Fact]
    public void Counting_sort_reads_and_writes_each_item_exactly_once()
    {
        int[] values = TestData.RandomInts(new Random(12), 1000, 0, 99);

        var counts = new CountingSort().Sort(values);

        Assert.Equal(1000, counts.Reads);
        Assert.Equal(1000, counts.Writes);
    }

    [Fact]
    public void Radix_sort_makes_one_pass_per_digit_of_the_key_range()
    {
        int[] values = [0, 999, .. TestData.RandomInts(new Random(13), 998, 0, 999)];

        var counts = new RadixSort().Sort(values);

        Assert.Equal(3 * values.Length, counts.Writes); // 0..999 has three decimal digits
    }

    /// <summary>
    /// Distinct values in the shapes that tend to break sorts: random, sorted, reversed, sawtooth,
    /// rotated (sorted with the smallest value moved to the end) and organ pipe (up, then down).
    /// </summary>
    private static IEnumerable<int[]> Shapes(int n)
    {
        int[] random = [.. Enumerable.Range(0, n)];
        new Random(n).Shuffle(random);
        yield return random;
        yield return [.. Enumerable.Range(0, n)];
        yield return [.. Enumerable.Range(0, n).Reverse()];
        yield return [.. Enumerable.Range(0, n).Select(i => (i % 64 * n) + i)];
        yield return [.. Enumerable.Range(1, n - 1), 0];
        yield return [.. Enumerable.Range(0, n / 2).Select(i => 2 * i), .. Enumerable.Range(0, n - (n / 2)).Select(i => (2 * i) + 1).Reverse()];
    }
}
