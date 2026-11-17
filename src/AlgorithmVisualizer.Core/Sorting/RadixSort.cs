namespace AlgorithmVisualizer.Core.Sorting;

public sealed class RadixSort : ISortAlgorithm
{
    private const int Base = 10;

    public AlgorithmInfo Info { get; } = new(
        "radix-sort",
        "Radix sort (LSD)",
        new Complexity("O(d·(n + b))", "O(d·(n + b))", "O(d·(n + b))", "O(n + b)"),
        "Sorts by the ones digit, then the tens, then the hundreds, using a stable counting pass each time, so the " +
        "order from earlier digits survives later ones. Cost grows with the number of digits d, not with n log n.")
    {
        IsStable = true,
        IsInPlace = false,
        NeedsIntegerKeys = true,
    };

    public void Sort<T>(TrackedArray<T> items)
    {
        items.RequireIntegerKey(Info.Name);
        int n = items.Length;
        if (n < 2)
        {
            return;
        }

        var held = new T[n];
        var keys = new long[n];
        long min = long.MaxValue;
        long max = long.MinValue;
        for (int i = 0; i < n; i++)
        {
            held[i] = items.Read(i);
            keys[i] = items.KeyOf(held[i]);
            min = Math.Min(min, keys[i]);
            max = Math.Max(max, keys[i]);
        }

        // Offsetting by the minimum makes every key non-negative, so negative numbers need no special case.
        // In long arithmetic max − min fits even for int.MinValue..int.MaxValue.
        for (int i = 0; i < n; i++)
        {
            keys[i] -= min;
        }

        long largest = max - min;
        var nextHeld = new T[n];
        var nextKeys = new long[n];
        var slots = new int[Base];
        for (long place = 1; largest / place > 0; place *= Base)
        {
            Array.Clear(slots);
            foreach (long key in keys)
            {
                slots[Digit(key, place)]++;
            }

            int position = 0;
            for (int d = 0; d < Base; d++)
            {
                int count = slots[d];
                slots[d] = position;
                position += count;
            }

            // A stable scatter by this digit. Writing straight into the array lets the visualizer show each pass.
            for (int i = 0; i < n; i++)
            {
                int slot = slots[Digit(keys[i], place)]++;
                nextHeld[slot] = held[i];
                nextKeys[slot] = keys[i];
                items.Write(slot, held[i]);
            }

            (held, nextHeld) = (nextHeld, held);
            (keys, nextKeys) = (nextKeys, keys);
        }
    }

    private static int Digit(long key, long place) => (int)(key / place % Base);
}
