using System.Diagnostics;
using AlgorithmVisualizer.Core.Tracing;

namespace AlgorithmVisualizer.Core.Playback;

/// <summary>
/// Replays a <see cref="RecordedRun{T}"/> one step at a time, forwards or backwards. Every step can be
/// undone (a swap swaps back, a write restores its <c>Previous</c> value), so scrubbing back and forth
/// never re-runs the algorithm.
/// </summary>
public sealed class TracePlayer<T>
{
    private readonly T[] _values;
    private readonly IReadOnlyList<Step<T>> _steps;
    private readonly int[] _sortedMarks;
    private readonly Stack<(int Low, int High)?> _windows = new();
    private long _comparisons;
    private long _swaps;
    private long _writes;
    private long _reads;

    public TracePlayer(RecordedRun<T> run)
        : this((run ?? throw new ArgumentNullException(nameof(run))).Input, run.Steps)
    {
    }

    public TracePlayer(IReadOnlyList<T> input, IReadOnlyList<Step<T>> steps)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(steps);
        _values = [.. input];
        _steps = steps;
        _sortedMarks = new int[_values.Length];
    }

    public IReadOnlyList<T> Values => _values;

    /// <summary>How many steps have been applied.</summary>
    public int Position { get; private set; }

    public int Length => _steps.Count;

    public bool IsAtStart => Position == 0;

    public bool IsAtEnd => Position == _steps.Count;

    public OperationCounts Counts => new(_comparisons, _swaps, _writes, _reads);

    /// <summary>The window a search is looking in, once it has narrowed one.</summary>
    public (int Low, int High)? Window { get; private set; }

    public int? FoundIndex { get; private set; }

    /// <summary>The most recent step that isn't a sorted mark: the one the visualizer highlights.</summary>
    public Step<T>? Highlight
    {
        get
        {
            for (int i = Position - 1; i >= 0; i--)
            {
                if (_steps[i].Kind != StepKind.MarkSorted)
                {
                    return _steps[i];
                }
            }

            return null;
        }
    }

    public bool IsMarkedSorted(int index) => _sortedMarks[index] > 0;

    /// <summary>Applies one step. Returns false at the end.</summary>
    public bool StepForward()
    {
        if (IsAtEnd)
        {
            return false;
        }

        Apply(_steps[Position]);
        Position++;
        return true;
    }

    /// <summary>Undoes one step. Returns false at the start.</summary>
    public bool StepBack()
    {
        if (IsAtStart)
        {
            return false;
        }

        Position--;
        Undo(_steps[Position]);
        return true;
    }

    /// <summary>
    /// Advances one visible step. Sorted marks that follow it are applied with it, since a mark on its
    /// own changes nothing worth a frame.
    /// </summary>
    public bool Next()
    {
        if (!StepForward())
        {
            return false;
        }

        while (!IsAtEnd && _steps[Position].Kind == StepKind.MarkSorted)
        {
            StepForward();
        }

        return true;
    }

    /// <summary>The reverse of <see cref="Next"/>.</summary>
    public bool Previous()
    {
        if (IsAtStart)
        {
            return false;
        }

        while (Position > 0 && _steps[Position - 1].Kind == StepKind.MarkSorted)
        {
            StepBack();
        }

        StepBack();
        return true;
    }

    /// <summary>Moves to <paramref name="position"/> (clamped to the trace) by stepping forwards or backwards.</summary>
    public void Seek(int position)
    {
        position = Math.Clamp(position, 0, _steps.Count);
        while (Position < position)
        {
            StepForward();
        }

        while (Position > position)
        {
            StepBack();
        }
    }

    private void Apply(in Step<T> step)
    {
        switch (step.Kind)
        {
            case StepKind.Compare:
                _comparisons++;
                break;
            case StepKind.Swap:
                (_values[step.First], _values[step.Second]) = (_values[step.Second], _values[step.First]);
                _swaps++;
                break;
            case StepKind.Write:
                _values[step.First] = step.Value!;
                _writes++;
                break;
            case StepKind.Read:
                _reads++;
                break;
            case StepKind.MarkSorted:
                _sortedMarks[step.First]++;
                break;
            case StepKind.Narrow:
                _windows.Push(Window);
                Window = (step.First, step.Second);
                break;
            case StepKind.Found:
                FoundIndex = step.First;
                break;
            default:
                throw new UnreachableException($"Unknown step kind {step.Kind}.");
        }
    }

    private void Undo(in Step<T> step)
    {
        switch (step.Kind)
        {
            case StepKind.Compare:
                _comparisons--;
                break;
            case StepKind.Swap:
                (_values[step.First], _values[step.Second]) = (_values[step.Second], _values[step.First]);
                _swaps--;
                break;
            case StepKind.Write:
                _values[step.First] = step.Previous!;
                _writes--;
                break;
            case StepKind.Read:
                _reads--;
                break;
            case StepKind.MarkSorted:
                _sortedMarks[step.First]--;
                break;
            case StepKind.Narrow:
                Window = _windows.Pop();
                break;
            case StepKind.Found:
                FoundIndex = null;
                break;
            default:
                throw new UnreachableException($"Unknown step kind {step.Kind}.");
        }
    }
}
