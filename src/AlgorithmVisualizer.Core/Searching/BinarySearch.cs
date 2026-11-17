namespace AlgorithmVisualizer.Core.Searching;

public sealed class BinarySearch : ISearchAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "binary-search",
        "Binary search",
        new Complexity("O(log n)", "O(log n)", "O(log n)", "O(1)"),
        "Halves a sorted window until one candidate is left. This is the lower-bound variant: it keeps going after a " +
        "match to find the first one, so results are predictable with duplicates. It gives up the early exit to get that.")
    {
        NeedsSortedInput = true,
    };

    public int Search<T>(TrackedArray<T> items, T target)
    {
        // Half-open window [low, high). Invariant: everything before low is < target,
        // everything from high on is >= target.
        int low = 0;
        int high = items.Length;
        while (low < high)
        {
            items.Narrow(low, high - 1);
            int mid = low + ((high - low) / 2); // (low + high) / 2 can overflow on huge arrays
            if (items.CompareTo(mid, target) < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        if (low < items.Length && items.CompareTo(low, target) == 0)
        {
            items.Found(low);
            return low;
        }

        return -1;
    }
}
