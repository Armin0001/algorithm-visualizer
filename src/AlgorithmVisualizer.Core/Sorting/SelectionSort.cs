namespace AlgorithmVisualizer.Core.Sorting;

public sealed class SelectionSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "selection-sort",
        "Selection sort",
        new Complexity("O(n²)", "O(n²)", "O(n²)", "O(1)"),
        "Finds the smallest remaining value and swaps it into the next slot. It always makes n²/2 comparisons, " +
        "but never more than n − 1 swaps, which matters when moving data costs more than comparing it.")
    {
        IsStable = false,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < n; j++)
            {
                if (items.Compare(j, min) < 0)
                {
                    min = j;
                }
            }

            // A long-distance swap can jump an element over its equals: this is why selection sort isn't stable.
            items.Swap(i, min);
            items.MarkSorted(i);
        }

        if (n > 0)
        {
            items.MarkSorted(n - 1);
        }
    }
}
