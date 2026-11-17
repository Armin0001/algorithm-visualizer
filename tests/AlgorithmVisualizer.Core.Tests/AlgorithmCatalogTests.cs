using AlgorithmVisualizer.Core.Searching;
using AlgorithmVisualizer.Core.Sorting;

namespace AlgorithmVisualizer.Core.Tests;

public class AlgorithmCatalogTests
{
    private static IEnumerable<AlgorithmInfo> AllInfo =>
        AlgorithmCatalog.Sorts.Select(a => a.Info).Concat(AlgorithmCatalog.Searches.Select(a => a.Info));

    [Fact]
    public void Ids_are_unique_and_url_friendly()
    {
        var ids = AllInfo.Select(i => i.Id).ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
        Assert.All(ids, id => Assert.Matches("^[a-z]+(-[a-z]+)*$", id));
    }

    [Fact]
    public void Every_algorithm_documents_its_costs_and_how_it_works()
    {
        foreach (var info in AllInfo)
        {
            string[] costs = [info.Complexity.Best, info.Complexity.Average, info.Complexity.Worst, info.Complexity.Space];

            Assert.False(string.IsNullOrWhiteSpace(info.Name));
            Assert.True(info.Summary.Length > 60, $"{info.Id} needs a real summary.");
            Assert.All(costs, cost => Assert.Contains("O(", cost, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Only_binary_and_jump_search_ask_for_sorted_input()
    {
        Assert.False(new LinearSearch().Info.NeedsSortedInput);
        Assert.True(new BinarySearch().Info.NeedsSortedInput);
        Assert.True(new JumpSearch().Info.NeedsSortedInput);
    }

    [Fact]
    public void Lookup_by_id_finds_the_algorithm_or_names_the_missing_id()
    {
        Assert.IsType<QuickSort>(AlgorithmCatalog.FindSort("quick-sort"));
        Assert.IsType<BinarySearch>(AlgorithmCatalog.FindSearch("binary-search"));

        var error = Assert.Throws<KeyNotFoundException>(() => AlgorithmCatalog.FindSort("bogo-sort"));
        Assert.Contains("bogo-sort", error.Message, StringComparison.Ordinal);
    }
}
