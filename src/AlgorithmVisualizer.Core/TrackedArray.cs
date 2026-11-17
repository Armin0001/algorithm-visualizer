using AlgorithmVisualizer.Core.Tracing;

namespace AlgorithmVisualizer.Core;

/// <summary>
/// The array every algorithm in this library works through. Each operation is counted and, when a
/// recorder is attached, recorded as a <see cref="Step{T}"/>. The algorithms never touch the
/// underlying array directly, so the counts and the replayed animation can't drift from what the
/// code really did.
/// </summary>
public sealed class TrackedArray<T>
{
    private readonly T[] _items;
    private readonly IComparer<T> _comparer;
    private readonly Func<T, int>? _integerKey;
    private readonly IStepRecorder<T>? _recorder;
    private long _comparisons;
    private long _swaps;
    private long _writes;
    private long _reads;

    /// <param name="items">The array to sort or search. Sorting happens in place.</param>
    /// <param name="comparer">Orders the items. Defaults to <see cref="Comparer{T}.Default"/>.</param>
    /// <param name="integerKey">
    /// The key the integer sorts (counting and radix) order by. They throw without one.
    /// </param>
    /// <param name="recorder">Optional. Receives every step, for replay.</param>
    public TrackedArray(T[] items, IComparer<T>? comparer = null, Func<T, int>? integerKey = null, IStepRecorder<T>? recorder = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items = items;
        _comparer = comparer ?? Comparer<T>.Default;
        _integerKey = integerKey;
        _recorder = recorder;
    }

    public int Length => _items.Length;

    public OperationCounts Counts => new(_comparisons, _swaps, _writes, _reads);

    public bool HasIntegerKey => _integerKey is not null;

    /// <summary>Compares the elements at <paramref name="i"/> and <paramref name="j"/>.</summary>
    public int Compare(int i, int j)
    {
        _comparisons++;
        _recorder?.Record(new Step<T>(StepKind.Compare, i, j));
        return _comparer.Compare(_items[i], _items[j]);
    }

    /// <summary>
    /// Compares the element at <paramref name="index"/> with a value held outside the array,
    /// such as a merge buffer entry or a search target.
    /// </summary>
    public int CompareTo(int index, T value)
    {
        _comparisons++;
        _recorder?.Record(new Step<T>(StepKind.Compare, index));
        return _comparer.Compare(_items[index], value);
    }

    /// <summary>Swaps two elements. Swapping an element with itself is a no-op and isn't counted.</summary>
    public void Swap(int i, int j)
    {
        if (i == j)
        {
            return;
        }

        _swaps++;
        _recorder?.Record(new Step<T>(StepKind.Swap, i, j));
        (_items[i], _items[j]) = (_items[j], _items[i]);
    }

    public T Read(int index)
    {
        _reads++;
        _recorder?.Record(new Step<T>(StepKind.Read, index));
        return _items[index];
    }

    public void Write(int index, T value)
    {
        _writes++;
        _recorder?.Record(new Step<T>(StepKind.Write, index, -1, value, _items[index]));
        _items[index] = value;
    }

    /// <summary>The integer key of a value, for the counting and radix sorts.</summary>
    public int KeyOf(T value) =>
        _integerKey is { } key
            ? key(value)
            : throw new InvalidOperationException("No integer key was given for these items.");

    /// <summary>Throws a clear error when an integer sort is handed items without an integer key.</summary>
    public void RequireIntegerKey(string algorithmName)
    {
        if (!HasIntegerKey)
        {
            throw new InvalidOperationException(
                $"{algorithmName} orders items by an integer key. Pass integerKey when sorting {typeof(T).Name} items (int arrays get one automatically).");
        }
    }

    /// <summary>Tells the visualizer <paramref name="index"/> holds its final value.</summary>
    public void MarkSorted(int index) => _recorder?.Record(new Step<T>(StepKind.MarkSorted, index));

    /// <summary>Tells the visualizer a search is now looking only at <paramref name="low"/>..<paramref name="high"/>.</summary>
    public void Narrow(int low, int high) => _recorder?.Record(new Step<T>(StepKind.Narrow, low, high));

    /// <summary>Tells the visualizer a search found its target at <paramref name="index"/>.</summary>
    public void Found(int index) => _recorder?.Record(new Step<T>(StepKind.Found, index));
}
