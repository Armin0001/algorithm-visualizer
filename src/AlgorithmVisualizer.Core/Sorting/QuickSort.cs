namespace AlgorithmVisualizer.Core.Sorting;

public sealed class QuickSort : ISortAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "quick-sort",
        "Quicksort",
        new Complexity("O(n)", "O(n log n)", "O(n²)", "O(log n)"),
        "Picks a pivot (median of three samples at the quartiles), then partitions three ways: smaller, equal, larger. " +
        "The equal block is final, so inputs with many repeats get faster, not slower, and an all-equal array takes one O(n) pass.")
    {
        IsStable = false,
        IsInPlace = true,
    };

    public void Sort<T>(TrackedArray<T> items) => SortRange(items, 0, items.Length - 1);

    private static void SortRange<T>(TrackedArray<T> items, int low, int high)
    {
        while (low < high)
        {
            if (high - low >= 2)
            {
                items.Swap(low, MedianOfThree(items, low, high));
            }

            // Dijkstra's three-way partition, pivot at items[low]:
            //   [low, lt) < pivot   [lt, i) == pivot   [i, gt] unknown   (gt, high] > pivot
            // The equal block is never empty, so items[lt] always holds a copy of the pivot.
            int lt = low;
            int i = low + 1;
            int gt = high;
            while (i <= gt)
            {
                int order = items.Compare(i, lt);
                if (order < 0)
                {
                    items.Swap(lt++, i++);
                }
                else if (order > 0)
                {
                    items.Swap(i, gt--);
                }
                else
                {
                    i++;
                }
            }

            for (int k = lt; k <= gt; k++)
            {
                items.MarkSorted(k);
            }

            // Recurse into the smaller side and loop on the larger one, so the stack never grows past O(log n).
            if (lt - low < high - gt)
            {
                SortRange(items, low, lt - 1);
                low = gt + 1;
            }
            else
            {
                SortRange(items, gt + 1, high);
                high = lt - 1;
            }
        }

        if (low == high)
        {
            items.MarkSorted(low);
        }
    }

    // A first- or last-element pivot makes sorted input quadratic, hence a median of three. The samples sit at
    // the quartiles rather than the textbook first/middle/last: the three-way partition leaves each side rotated
    // by one (its smallest value ends up last), so an end sample is nearly always an extreme. With end samples,
    // sorted input cost 4n log2 n comparisons at n = 4096 and kept climbing with n; ComplexityTests caught it.
    private static int MedianOfThree<T>(TrackedArray<T> items, int low, int high)
    {
        int quarter = (high - low) / 4;
        int a = low + quarter;
        int b = low + ((high - low) / 2);
        int c = high - quarter;

        if (items.Compare(a, b) < 0)
        {
            if (items.Compare(b, c) < 0)
            {
                return b; // a < b < c
            }

            return items.Compare(a, c) < 0 ? c : a;
        }

        if (items.Compare(a, c) < 0)
        {
            return a; // b <= a < c
        }

        return items.Compare(b, c) < 0 ? c : b;
    }
}
