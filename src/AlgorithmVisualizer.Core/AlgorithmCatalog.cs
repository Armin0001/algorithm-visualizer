using AlgorithmVisualizer.Core.Searching;
using AlgorithmVisualizer.Core.Sorting;

namespace AlgorithmVisualizer.Core;

/// <summary>Every algorithm the app offers, in the order the UI lists them. The tests run against this list too.</summary>
public static class AlgorithmCatalog
{
    public static IReadOnlyList<ISortAlgorithm> Sorts { get; } =
    [
        new BubbleSort(),
        new SelectionSort(),
        new InsertionSort(),
        new ShellSort(),
        new MergeSort(),
        new QuickSort(),
        new HeapSort(),
        new CountingSort(),
        new RadixSort(),
    ];

    public static IReadOnlyList<ISearchAlgorithm> Searches { get; } =
    [
        new LinearSearch(),
        new BinarySearch(),
        new JumpSearch(),
    ];

    public static ISortAlgorithm FindSort(string id) =>
        Sorts.FirstOrDefault(a => a.Info.Id == id) ?? throw new KeyNotFoundException($"No sorting algorithm has the id '{id}'.");

    public static ISearchAlgorithm FindSearch(string id) =>
        Searches.FirstOrDefault(a => a.Info.Id == id) ?? throw new KeyNotFoundException($"No search algorithm has the id '{id}'.");
}
