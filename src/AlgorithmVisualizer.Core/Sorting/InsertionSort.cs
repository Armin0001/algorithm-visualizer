namespace AlgorithmVisualizer.Core.Sorting;

public sealed class InsertionSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "insertion-sort",
        "Insertion sort",
        new Complexity("O(n)", "O(n²)", "O(n²)", "O(1)"),
        "Grows a sorted prefix one element at a time, swapping each new element left until it fits. " +
        "It does one swap per inversion, so nearly sorted input finishes in close to linear time.")
    {
        IsStable = true,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        for (int i = 1; i < items.Length; i++)
        {
            // Strictly greater: equal elements never pass each other, which keeps the sort stable.
            for (int j = i; j > 0 && items.Compare(j - 1, j) > 0; j--)
            {
                items.Swap(j - 1, j);
            }
        }
    }
}
