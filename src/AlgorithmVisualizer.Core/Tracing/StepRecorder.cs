namespace AlgorithmVisualizer.Core.Tracing;

/// <summary>Receives every step a <see cref="TrackedArray{T}"/> performs.</summary>
public interface IStepRecorder<T>
{
    void Record(in Step<T> step);
}

/// <summary>Keeps every step in memory, in order, so the run can be replayed.</summary>
public sealed class StepRecorder<T> : IStepRecorder<T>
{
    private readonly List<Step<T>> _steps = [];

    public IReadOnlyList<Step<T>> Steps => _steps;

    public void Record(in Step<T> step) => _steps.Add(step);
}
