namespace AlgorithmVisualizer.Core.Searching;

public sealed class JumpSearch : ISearchAlgorithm
{
    public AlgorithmInfo Info { get; } = new(
        "jump-search",
        "Jump search",
        new Complexity("O(1)", "O(√n)", "O(√n)", "O(1)"),
        "Jumps ahead √n elements at a time until it passes the target, then steps back once to scan that one block. " +
        "Slower than binary search, but it steps backwards only that once, which suits data that is expensive to seek backwards in.")
    {
        NeedsSortedInput = true,
    };

    public int Search<T>(TrackedArray<T> items, T target)
    {
        int n = items.Length;
        if (n == 0)
        {
            return -1;
        }

        int step = Math.Max(1, (int)Math.Sqrt(n));
        int blockStart = 0;
        int blockEnd = Math.Min(step, n); // exclusive

        // Find the first block whose last element is >= target. Every element before it is < target,
        // so the first match, if there is one, is inside that block.
        while (items.CompareTo(blockEnd - 1, target) < 0)
        {
            blockStart = blockEnd;
            if (blockStart >= n)
            {
                return -1;
            }

            blockEnd = Math.Min(blockEnd + step, n);
        }

        items.Narrow(blockStart, blockEnd - 1);
        for (int i = blockStart; i < blockEnd; i++)
        {
            int order = items.CompareTo(i, target);
            if (order == 0)
            {
                items.Found(i);
                return i;
            }

            if (order > 0)
            {
                return -1; // Passed the place it would be.
            }
        }

        return -1;
    }
}
