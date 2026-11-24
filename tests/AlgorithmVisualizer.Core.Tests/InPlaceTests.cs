namespace AlgorithmVisualizer.Core.Tests;

/// <summary>
/// The UI labels every sort "In place" or "Uses extra memory", so the label is tested in both directions,
/// the way stability is: a sort marked in place may not allocate a buffer that grows with the input, and one
/// marked otherwise must be caught allocating one.
/// </summary>
public class InPlaceTests
{
    private const int Length = 8192;

    [Theory]
    [MemberData(nameof(TestData.InPlaceSorts), MemberType = typeof(TestData))]
    public void Sorts_marked_in_place_allocate_nothing_that_grows_with_the_input(string id)
    {
        long allocated = BytesAllocatedSorting(id);

        // A buffer of even an eighth of the input would be 4 KB. Recursion lives on the stack, not the heap.
        Assert.True(allocated < 1024, $"{id} is marked in place, but sorting {Length} ints allocated {allocated} bytes.");
    }

    [Theory]
    [MemberData(nameof(TestData.OutOfPlaceSorts), MemberType = typeof(TestData))]
    public void Sorts_marked_as_using_extra_memory_really_do_allocate_a_buffer(string id)
    {
        long allocated = BytesAllocatedSorting(id);

        // Merge sort's half-size buffer is the smallest here: Length / 2 ints.
        long halfBuffer = Length / 2 * sizeof(int);
        Assert.True(allocated >= halfBuffer, $"{id} is marked as using extra memory, but sorting {Length} ints allocated only {allocated} bytes.");
    }

    /// <summary>Heap bytes allocated by the sort itself, with the tracked array built beforehand and the code already JIT-compiled.</summary>
    private static long BytesAllocatedSorting(string id)
    {
        var algorithm = TestData.Sort(id);
        algorithm.Sort(TestData.RandomInts(new Random(1), 64, 0, 1_000)); // warm up, so first-call costs aren't counted

        var tracked = new TrackedArray<int>(TestData.RandomInts(new Random(2), Length, 0, 1_000), integerKey: static x => x);
        long before = GC.GetAllocatedBytesForCurrentThread();
        algorithm.Sort(tracked);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
