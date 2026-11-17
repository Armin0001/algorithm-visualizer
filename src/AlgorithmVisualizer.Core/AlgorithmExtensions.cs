using AlgorithmVisualizer.Core.Searching;
using AlgorithmVisualizer.Core.Sorting;
using AlgorithmVisualizer.Core.Tracing;

namespace AlgorithmVisualizer.Core;

/// <summary>A run captured for replay: where it started, every step it took, and what it produced.</summary>
public sealed record RecordedRun<T>(
    IReadOnlyList<T> Input,
    IReadOnlyList<T> Output,
    IReadOnlyList<Step<T>> Steps,
    OperationCounts Counts,
    int? FoundIndex = null);

/// <summary>Convenience entry points, so callers don't have to build a <see cref="TrackedArray{T}"/> by hand.</summary>
public static class AlgorithmExtensions
{
    private static readonly Func<int, int> Identity = static x => x;

    /// <summary>Sorts an int array in place and returns how much work that took.</summary>
    public static OperationCounts Sort(this ISortAlgorithm algorithm, int[] values) =>
        algorithm.Sort(values, comparer: null, integerKey: Identity);

    /// <summary>Sorts any array in place. The integer sorts (counting, radix) need <paramref name="integerKey"/>.</summary>
    public static OperationCounts Sort<T>(this ISortAlgorithm algorithm, T[] items, IComparer<T>? comparer = null, Func<T, int>? integerKey = null)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        var tracked = new TrackedArray<T>(items, comparer, integerKey);
        algorithm.Sort(tracked);
        return tracked.Counts;
    }

    /// <summary>Sorts a copy of <paramref name="input"/>, recording every step for replay.</summary>
    public static RecordedRun<int> Record(this ISortAlgorithm algorithm, IReadOnlyList<int> input)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        int[] values = [.. input];
        var recorder = new StepRecorder<int>();
        var tracked = new TrackedArray<int>(values, integerKey: Identity, recorder: recorder);
        algorithm.Sort(tracked);
        return new RecordedRun<int>([.. input], values, recorder.Steps, tracked.Counts);
    }

    /// <summary>Searches an int array and returns the first matching index, or -1.</summary>
    public static int Search(this ISearchAlgorithm algorithm, int[] values, int target)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        return algorithm.Search(new TrackedArray<int>(values), target);
    }

    /// <summary>Searches <paramref name="input"/> for <paramref name="target"/>, recording every step for replay.</summary>
    public static RecordedRun<int> Record(this ISearchAlgorithm algorithm, IReadOnlyList<int> input, int target)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        int[] values = [.. input];
        var recorder = new StepRecorder<int>();
        var tracked = new TrackedArray<int>(values, recorder: recorder);
        int index = algorithm.Search(tracked, target);
        return new RecordedRun<int>(values, values, recorder.Steps, tracked.Counts, index >= 0 ? index : null);
    }
}
