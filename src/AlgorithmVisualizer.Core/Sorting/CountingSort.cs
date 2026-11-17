namespace AlgorithmVisualizer.Core.Sorting;

public sealed class CountingSort : ISortAlgorithm
{
    /// <summary>
    /// The widest key range (max − min + 1) it will allocate counters for: about 4 MB of counts.
    /// Wider ranges are what radix sort is for.
    /// </summary>
    public const int MaxKeyRange = 1 << 20;

    public AlgorithmInfo Info { get; } = new(
        "counting-sort",
        "Counting sort",
        new Complexity("O(n + k)", "O(n + k)", "O(n + k)", "O(n + k)"),
        "Counts how often each key occurs, turns the counts into starting positions, then writes every item " +
        "straight to its final slot. It never compares two items, so it beats the O(n log n) limit when the key range k is small.")
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
        var keys = new int[n];
        int min = int.MaxValue;
        int max = int.MinValue;
        for (int i = 0; i < n; i++)
        {
            held[i] = items.Read(i);
            keys[i] = items.KeyOf(held[i]);
            min = Math.Min(min, keys[i]);
            max = Math.Max(max, keys[i]);
        }

        long range = (long)max - min + 1;
        if (range > MaxKeyRange)
        {
            throw new ArgumentOutOfRangeException(
                nameof(items),
                $"Counting sort needs a counter per possible key, and these keys span {range:N0} values (the limit is {MaxKeyRange:N0}). Use radix sort instead.");
        }

        var next = new int[range];
        foreach (int key in keys)
        {
            next[key - min]++;
        }

        // Turn counts into the first free slot for each key.
        int position = 0;
        for (int b = 0; b < next.Length; b++)
        {
            int count = next[b];
            next[b] = position;
            position += count;
        }

        // Walking the input in order and filling each key's slots left to right keeps equal keys in input order.
        for (int i = 0; i < n; i++)
        {
            int slot = next[keys[i] - min]++;
            items.Write(slot, held[i]);
            items.MarkSorted(slot);
        }
    }
}
