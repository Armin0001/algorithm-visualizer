namespace AlgorithmVisualizer.Core.Tracing;

/// <summary>The kind of operation a <see cref="Step{T}"/> records.</summary>
public enum StepKind
{
    /// <summary>
    /// Two elements were compared. <c>Second</c> is -1 when one side was a value held outside
    /// the array (a merge buffer or a search target).
    /// </summary>
    Compare,

    /// <summary>The elements at <c>First</c> and <c>Second</c> swapped places.</summary>
    Swap,

    /// <summary>
    /// <c>First</c> was overwritten with <c>Value</c>. <c>Previous</c> keeps the old value so the
    /// step can be undone.
    /// </summary>
    Write,

    /// <summary><c>First</c> was read without a comparison, for example to count its key.</summary>
    Read,

    /// <summary><c>First</c> now holds its final value. A hint for the visualizer; it changes no data.</summary>
    MarkSorted,

    /// <summary>A search narrowed its window to the inclusive range <c>First</c>..<c>Second</c>.</summary>
    Narrow,

    /// <summary>A search found its target at <c>First</c>.</summary>
    Found,
}

/// <summary>
/// One recorded operation. A run is a list of these, and replaying them in order rebuilds the run.
/// </summary>
public readonly record struct Step<T>(StepKind Kind, int First, int Second = -1, T? Value = default, T? Previous = default);
