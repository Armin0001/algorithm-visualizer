namespace AlgorithmVisualizer.Core.Sorting;

public sealed class ShellSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "shell-sort",
        "Shell sort",
        new Complexity("O(n log n)", "≈ O(n^1.25)", "O(n^1.5)", "O(1)"),
        "Insertion sort over shrinking gaps (Knuth's 1, 4, 13, 40, …). The early, wide passes move values long " +
        "distances cheaply, so the final gap-1 pass only has short distances left to fix.")
    {
        IsStable = false,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        int n = items.Length;

        // Knuth's sequence has a proven O(n^1.5) worst case, unlike Shell's original n/2, n/4, … gaps (O(n²)).
        int gap = 1;
        while (gap < n / 3)
        {
            gap = (3 * gap) + 1;
        }

        for (; gap >= 1; gap /= 3)
        {
            for (int i = gap; i < n; i++)
            {
                for (int j = i; j >= gap && items.Compare(j - gap, j) > 0; j -= gap)
                {
                    items.Swap(j - gap, j);
                }
            }
        }
    }
}
