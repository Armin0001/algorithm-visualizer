namespace AlgorithmVisualizer.Core;

/// <summary>How much work a run did. Comparisons and data movement are counted apart because they
/// cost differently: counting sort makes no comparisons at all, and selection sort makes few swaps.</summary>
public readonly record struct OperationCounts(long Comparisons, long Swaps, long Writes, long Reads);

/// <summary>Big-O costs as usually quoted: time in the best, average and worst case, then extra space.</summary>
public sealed record Complexity(string Best, string Average, string Worst, string Space);

/// <summary>
/// What the UI and the docs say about an algorithm. The tests hold these flags to account: a sort
/// marked stable is checked for stability, and one marked unstable must be caught reordering equal keys.
/// </summary>
public sealed record AlgorithmInfo(string Id, string Name, Complexity Complexity, string Summary)
{
    public bool IsStable { get; init; }

    public bool IsInPlace { get; init; }

    public bool NeedsSortedInput { get; init; }

    public bool NeedsIntegerKeys { get; init; }
}
