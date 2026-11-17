namespace AlgorithmVisualizer.Core.Inputs;

/// <summary>
/// The starting arrangements the visualizer offers. The shape matters as much as the size: insertion sort
/// flies through nearly sorted input, a naive quicksort collapses on sorted input, and few unique values
/// show off three-way partitioning.
/// </summary>
public enum InputShape
{
    Random,
    NearlySorted,
    Reversed,
    FewUnique,
    Sorted,
}

public static class InputGenerator
{
    /// <summary>Smallest generated value. Kept above zero so every bar stays visible.</summary>
    public const int MinValue = 5;

    public const int MaxValue = 100;

    private static readonly int[] FewUniqueValues = [20, 45, 70, 95];

    /// <summary>Builds an array of <paramref name="length"/> values. The same seed always gives the same array.</summary>
    public static int[] Create(InputShape shape, int length, int seed)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var random = new Random(seed);
        var values = new int[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = random.Next(MinValue, MaxValue + 1);
        }

        switch (shape)
        {
            case InputShape.Random:
                break;
            case InputShape.Sorted:
                Array.Sort(values);
                break;
            case InputShape.Reversed:
                Array.Sort(values);
                Array.Reverse(values);
                break;
            case InputShape.NearlySorted:
                Array.Sort(values);
                DisturbSlightly(values, random);
                break;
            case InputShape.FewUnique:
                for (int i = 0; i < length; i++)
                {
                    values[i] = FewUniqueValues[random.Next(FewUniqueValues.Length)];
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shape), shape, "Unknown input shape.");
        }

        return values;
    }

    // About one element in ten is swapped with a neighbour at most three places away.
    private static void DisturbSlightly(int[] values, Random random)
    {
        if (values.Length < 2)
        {
            return;
        }

        int swaps = Math.Max(1, values.Length / 10);
        for (int s = 0; s < swaps; s++)
        {
            int i = random.Next(values.Length);
            int j = Math.Clamp(i + random.Next(-3, 4), 0, values.Length - 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }
}
