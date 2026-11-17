namespace AlgorithmVisualizer.Core.Sorting;

public sealed class HeapSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "heap-sort",
        "Heap sort",
        new Complexity("O(n log n)", "O(n log n)", "O(n log n)", "O(1)"),
        "Builds a max-heap inside the array, then repeatedly swaps the largest value to the end and repairs the heap. " +
        "A guaranteed O(n log n) with no extra memory, at the price of poor cache locality and no stability.")
    {
        IsStable = false,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        int n = items.Length;

        // Bottom-up heap construction is O(n), cheaper than inserting the elements one at a time.
        for (int start = (n / 2) - 1; start >= 0; start--)
        {
            SiftDown(items, start, n);
        }

        for (int end = n - 1; end > 0; end--)
        {
            items.Swap(0, end);
            items.MarkSorted(end);
            SiftDown(items, 0, end);
        }

        if (n > 0)
        {
            items.MarkSorted(0);
        }
    }

    private static void SiftDown<T>(TrackedArray<T> items, int root, int length)
    {
        while (true)
        {
            int child = (2 * root) + 1;
            if (child >= length)
            {
                return;
            }

            if (child + 1 < length && items.Compare(child + 1, child) > 0)
            {
                child++;
            }

            if (items.Compare(child, root) <= 0)
            {
                return;
            }

            items.Swap(root, child);
            root = child;
        }
    }
}
