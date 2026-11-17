namespace AlgorithmVisualizer.Core.Searching;

public sealed class LinearSearch : ISearchAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "linear-search",
        "Linear search",
        new Complexity("O(1)", "O(n)", "O(n)", "O(1)"),
        "Checks every element from the left until one matches. The only search here that works on unsorted data, " +
        "and on small arrays often the fastest in practice.");

    public int Search<T>(TrackedArray<T> items, T target)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items.CompareTo(i, target) == 0)
            {
                items.Found(i);
                return i;
            }
        }

        return -1;
    }
}
