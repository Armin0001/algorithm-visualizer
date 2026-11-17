namespace AlgorithmVisualizer.Core.Searching;

/// <summary>
/// A search algorithm. Every implementation returns the <em>first</em> matching index, so on sorted
/// input they all agree with each other and with <see cref="Array.IndexOf{T}(T[], T)"/>.
/// </summary>
public interface ISearchAlgorithm
{
    AlgorithmInfo Info { get; }

    /// <summary>Returns the index of the first element equal to <paramref name="target"/>, or -1 if there is none.</summary>
    int Search<T>(TrackedArray<T> items, T target);
}
