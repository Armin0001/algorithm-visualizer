namespace AlgorithmVisualizer.Core.Sorting;

public sealed class BubbleSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "bubble-sort",
        "Bubble sort",
        new Complexity("O(n)", "O(n²)", "O(n²)", "O(1)"),
        "Walks the array swapping out-of-order neighbours, so each pass carries the largest remaining value to the end. " +
        "It remembers where the last swap happened: everything after it is already in place, and a pass with no swaps ends the sort.")
    {
        IsStable = true,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        int end = items.Length - 1;
        while (end > 0)
        {
            int lastSwap = 0;
            for (int i = 0; i < end; i++)
            {
                if (items.Compare(i, i + 1) > 0)
                {
                    items.Swap(i, i + 1);
                    lastSwap = i;
                }
            }

            for (int k = lastSwap + 1; k <= end; k++)
            {
                items.MarkSorted(k);
            }

            end = lastSwap;
        }

        if (items.Length > 0)
        {
            items.MarkSorted(0);
        }
    }
}
