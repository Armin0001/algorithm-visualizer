using AlgorithmVisualizer.Core.Inputs;
using AlgorithmVisualizer.Core.Playback;
using AlgorithmVisualizer.Core.Searching;
using AlgorithmVisualizer.Core.Sorting;
using AlgorithmVisualizer.Core.Tracing;

namespace AlgorithmVisualizer.Core.Tests;

/// <summary>
/// The animation is only honest if replaying the recorded steps lands exactly where the algorithm did.
/// These tests check that for every algorithm and every input shape, forwards and backwards.
/// </summary>
public class TracePlayerTests
{
    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Replaying_a_recorded_sort_rebuilds_its_result_and_its_counts(string id)
    {
        foreach (var shape in Enum.GetValues<InputShape>())
        {
            int[] input = InputGenerator.Create(shape, 60, seed: 4);
            var run = TestData.Sort(id).Record(input);
            var player = new TracePlayer<int>(run);

            player.Seek(player.Length);

            Assert.True(player.IsAtEnd);
            Assert.Equal(input.Order(), player.Values);
            Assert.Equal(run.Output, player.Values);
            Assert.Equal(run.Counts, player.Counts);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Stepping_all_the_way_back_restores_the_input(string id)
    {
        int[] input = InputGenerator.Create(InputShape.Random, 60, seed: 5);
        var player = new TracePlayer<int>(TestData.Sort(id).Record(input));
        player.Seek(player.Length);

        while (player.StepBack())
        {
        }

        Assert.Equal(input, player.Values);
        Assert.Equal(default(OperationCounts), player.Counts);
        Assert.All(Enumerable.Range(0, input.Length), i => Assert.False(player.IsMarkedSorted(i)));
    }

    [Theory]
    [MemberData(nameof(TestData.AllSorts), MemberType = typeof(TestData))]
    public void Recording_does_not_change_what_the_algorithm_does(string id)
    {
        int[] input = InputGenerator.Create(InputShape.Random, 80, seed: 6);
        int[] silent = [.. input];

        var run = TestData.Sort(id).Record(input);
        var counts = TestData.Sort(id).Sort(silent);

        Assert.Equal(silent, run.Output);
        Assert.Equal(counts, run.Counts);
        Assert.Equal(input, run.Input); // recording works on a copy
    }

    [Fact]
    public void Seeking_lands_on_the_same_state_as_stepping()
    {
        var run = new QuickSort().Record(InputGenerator.Create(InputShape.Random, 50, seed: 7));
        var stepped = new TracePlayer<int>(run);
        var seeked = new TracePlayer<int>(run);

        for (int i = 0; i < 100; i++)
        {
            stepped.StepForward();
        }

        seeked.Seek(run.Steps.Count);
        seeked.Seek(100);

        Assert.Equal(stepped.Values, seeked.Values);
        Assert.Equal(stepped.Counts, seeked.Counts);
        Assert.Equal(stepped.Highlight, seeked.Highlight);
    }

    [Fact]
    public void Next_and_Previous_move_one_visible_step_and_carry_sorted_marks_with_them()
    {
        // Bubble sort on [3, 1, 2] records:
        // Compare(0,1) Swap(0,1) Compare(1,2) Swap(1,2) Mark(2) Compare(0,1) Mark(1) Mark(0)
        var run = new BubbleSort().Record([3, 1, 2]);
        var player = new TracePlayer<int>(run);
        Assert.Equal(8, run.Steps.Count);

        for (int i = 0; i < 4; i++)
        {
            player.Next();
        }

        Assert.Equal(5, player.Position);
        Assert.Equal(new Step<int>(StepKind.Swap, 1, 2), player.Highlight);
        Assert.True(player.IsMarkedSorted(2));

        player.Next();
        Assert.True(player.IsAtEnd);

        player.Previous();
        int[] expected = [1, 2, 3];
        Assert.Equal(5, player.Position);
        Assert.Equal(expected, player.Values);
        Assert.True(player.IsMarkedSorted(2));
        Assert.False(player.IsMarkedSorted(1));
    }

    /// <summary>
    /// The sorts are replayed through the catalog, but the searches only had the two hand-written cases
    /// below: linear search was never replayed at all, and a fourth search would arrive with no replay
    /// coverage. This runs every search in the catalog, forwards and backwards.
    /// </summary>
    [Theory]
    [MemberData(nameof(TestData.AllSearches), MemberType = typeof(TestData))]
    public void Replaying_a_recorded_search_rebuilds_its_match_and_its_counts(string id)
    {
        // Sorted, with a run of equal values, so the replay covers "the first match" too.
        int[] values = [1, 3, 3, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27];
        int[] targets = [1, 3, 27, 0, 4, 99]; // first, a duplicate, last, then below, in a gap and above

        foreach (int target in targets)
        {
            int match = Array.IndexOf(values, target);
            int? expected = match >= 0 ? match : null;
            var run = TestData.Search(id).Record(values, target);
            var player = new TracePlayer<int>(run);

            Assert.Equal(expected, run.FoundIndex);

            player.Seek(player.Length);

            Assert.True(player.IsAtEnd);
            Assert.Equal(run.FoundIndex, player.FoundIndex);
            Assert.Equal(run.Counts, player.Counts);
            Assert.Equal(values, player.Values); // a search compares, but never writes

            while (player.StepBack())
            {
            }

            Assert.Equal(default(OperationCounts), player.Counts);
            Assert.Null(player.Window);
            Assert.Null(player.FoundIndex);
        }
    }

    [Fact]
    public void Search_replay_tracks_the_window_and_the_match()
    {
        int[] sorted = [.. Enumerable.Range(0, 32).Select(i => i * 3)];
        var run = new BinarySearch().Record(sorted, 42);
        var player = new TracePlayer<int>(run);
        Assert.Equal(14, run.FoundIndex);

        player.Next();
        Assert.Equal((0, 31), player.Window);

        player.Seek(player.Length);
        Assert.Equal(14, player.FoundIndex);

        player.Seek(0);
        Assert.Null(player.Window);
        Assert.Null(player.FoundIndex);
    }

    [Fact]
    public void A_missed_search_records_no_match()
    {
        var run = new JumpSearch().Record([1, 3, 5, 7], 4);

        Assert.Null(run.FoundIndex);
        Assert.DoesNotContain(run.Steps, step => step.Kind == StepKind.Found);
    }
}
