namespace AlgorithmVisualizer.Core.Sorting;

/// <summary>
/// A sorting algorithm, written once against <see cref="TrackedArray{T}"/> so the same code runs
/// silently in the tests and step by step in the visualizer.
/// </summary>
public interface ISortAlgorithm
{
    AlgorithmInfo Info { get; }

    /// <summary>Sorts <paramref name="items"/> into ascending order, in place.</summary>
    void Sort<T>(TrackedArray<T> items);
}
