using AlgorithmVisualizer.Core.Searching;
using AlgorithmVisualizer.Core.Sorting;

namespace AlgorithmVisualizer.Core.Tests;

/// <summary>
/// Theory data built from <see cref="AlgorithmCatalog"/>, so a new algorithm is put through every
/// test the moment it is added to the catalog.
/// </summary>
public static class TestData
{
    public static TheoryData<string> AllSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info));

    public static TheoryData<string> StableSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => i.IsStable));

    public static TheoryData<string> UnstableSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => !i.IsStable));

    public static TheoryData<string> InPlaceSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => i.IsInPlace));

    public static TheoryData<string> OutOfPlaceSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => !i.IsInPlace));

    public static TheoryData<string> ComparisonSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => !i.NeedsIntegerKeys));

    public static TheoryData<string> IntegerSorts => Ids(AlgorithmCatalog.Sorts.Select(a => a.Info).Where(i => i.NeedsIntegerKeys));

    /// <summary>Every sort except counting sort, which by design refuses key ranges it can't allocate counters for.</summary>
    public static TheoryData<string> FullRangeSorts => Ids(AlgorithmCatalog.Sorts.Where(a => a is not CountingSort).Select(a => a.Info));

    public static TheoryData<string> AllSearches => Ids(AlgorithmCatalog.Searches.Select(a => a.Info));

    public static TheoryData<string> SortedInputSearches => Ids(AlgorithmCatalog.Searches.Select(a => a.Info).Where(i => i.NeedsSortedInput));

    public static TheoryData<string> AnyOrderSearches => Ids(AlgorithmCatalog.Searches.Select(a => a.Info).Where(i => !i.NeedsSortedInput));

    public static ISortAlgorithm Sort(string id) => AlgorithmCatalog.FindSort(id);

    public static ISearchAlgorithm Search(string id) => AlgorithmCatalog.FindSearch(id);

    /// <summary>Uniform random ints in [min, max], both ends included, even at the int extremes.</summary>
    public static int[] RandomInts(Random random, int length, int min, int max)
    {
        var values = new int[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = (int)random.NextInt64(min, (long)max + 1);
        }

        return values;
    }

    /// <summary>Pairs (i, j) with i &lt; j and values[i] &gt; values[j], counted the slow, obviously correct way.</summary>
    public static long CountInversions(int[] values)
    {
        long inversions = 0;
        for (int i = 0; i < values.Length; i++)
        {
            for (int j = i + 1; j < values.Length; j++)
            {
                if (values[i] > values[j])
                {
                    inversions++;
                }
            }
        }

        return inversions;
    }

    private static TheoryData<string> Ids(IEnumerable<AlgorithmInfo> infos)
    {
        var data = new TheoryData<string>();
        foreach (var info in infos)
        {
            data.Add(info.Id);
        }

        return data;
    }
}
