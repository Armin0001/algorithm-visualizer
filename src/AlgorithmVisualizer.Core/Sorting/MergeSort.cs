namespace AlgorithmVisualizer.Core.Sorting;

public sealed class MergeSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "merge-sort",
        "Merge sort",
        new Complexity("O(n)", "O(n log n)", "O(n log n)", "O(n)"),
        "Sorts each half, then merges them. Before merging it checks whether the halves are already in order " +
        "and skips the merge if so, which makes sorted input a single O(n) pass.")
    {
        IsStable = true,
        IsInPlace = false,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        if (items.Length < 2)
        {
            return;
        }

        // Only the left run is ever copied out (see Merge), so half the array is enough buffer.
        var buffer = new T[(items.Length + 1) / 2];
        SortRange(items, buffer, 0, items.Length - 1);
    }

    private static void SortRange<T>(TrackedArray<T> items, T[] buffer, int low, int high)
    {
        if (low >= high)
        {
            return;
        }

        int mid = low + ((high - low) / 2);
        SortRange(items, buffer, low, mid);
        SortRange(items, buffer, mid + 1, high);

        if (items.Compare(mid, mid + 1) <= 0)
        {
            return; // The halves are already in order.
        }

        Merge(items, buffer, low, mid, high);
    }

    private static void Merge<T>(TrackedArray<T> items, T[] buffer, int low, int mid, int high)
    {
        // Copy out the left run only. The right run can stay where it is: the write position never
        // overtakes the right-hand read position, so nothing unread gets overwritten.
        int leftLength = mid - low + 1;
        for (int x = 0; x < leftLength; x++)
        {
            buffer[x] = items.Read(low + x);
        }

        int i = 0;
        int j = mid + 1;
        int k = low;
        while (i < leftLength && j <= high)
        {
            // Take from the right only when it is strictly smaller. On ties the left element goes first,
            // and that is what makes merge sort stable.
            if (items.CompareTo(j, buffer[i]) < 0)
            {
                items.Write(k++, items.Read(j++));
            }
            else
            {
                items.Write(k++, buffer[i++]);
            }
        }

        while (i < leftLength)
        {
            items.Write(k++, buffer[i++]);
        }

        // Anything left in the right run is already in its final place.
    }
}
