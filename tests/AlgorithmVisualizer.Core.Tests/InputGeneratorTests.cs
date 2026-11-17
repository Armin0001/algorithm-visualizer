using AlgorithmVisualizer.Core.Inputs;

namespace AlgorithmVisualizer.Core.Tests;

public class InputGeneratorTests
{
    [Fact]
    public void The_same_seed_always_gives_the_same_array()
    {
        foreach (var shape in Enum.GetValues<InputShape>())
        {
            Assert.Equal(InputGenerator.Create(shape, 100, seed: 9), InputGenerator.Create(shape, 100, seed: 9));
        }
    }

    [Fact]
    public void Values_stay_inside_the_range_the_chart_can_draw()
    {
        foreach (var shape in Enum.GetValues<InputShape>())
        {
            Assert.All(InputGenerator.Create(shape, 500, seed: 1), v => Assert.InRange(v, InputGenerator.MinValue, InputGenerator.MaxValue));
        }
    }

    [Fact]
    public void Sorted_and_reversed_inputs_are_in_order()
    {
        int[] sorted = InputGenerator.Create(InputShape.Sorted, 200, seed: 2);
        int[] reversed = InputGenerator.Create(InputShape.Reversed, 200, seed: 2);

        Assert.Equal(sorted.Order(), sorted);
        Assert.Equal(reversed.OrderDescending(), reversed);
    }

    [Fact]
    public void Nearly_sorted_input_has_far_fewer_inversions_than_random_input()
    {
        int[] nearly = InputGenerator.Create(InputShape.NearlySorted, 200, seed: 3);
        int[] random = InputGenerator.Create(InputShape.Random, 200, seed: 3);

        // Each short swap adds at most five inversions; a random array of 200 has about 10,000.
        Assert.InRange(TestData.CountInversions(nearly), 0, 200);
        Assert.True(TestData.CountInversions(random) > 5_000);
    }

    [Fact]
    public void Few_unique_input_uses_at_most_four_distinct_values()
    {
        int[] values = InputGenerator.Create(InputShape.FewUnique, 300, seed: 4);

        Assert.InRange(values.Distinct().Count(), 1, 4);
    }

    [Fact]
    public void A_negative_length_is_rejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => InputGenerator.Create(InputShape.Random, -1, seed: 0));
}
