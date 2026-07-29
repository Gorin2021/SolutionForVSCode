using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class EdgeWithWeightBenchmark
{
    private Graph<int> _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        _graph = new Graph<int>();

        for (int i = 0; i < 1_000_001; i++)
            _graph.TryAddVertex(i);
    }

    [Benchmark]
    public void AddMillionEdges()
    {
        for (int i = 0; i < 1_000_000; i++)
            _graph.TryAddEdge(i, i + 1);
    }

    [Benchmark]
    public void AddExistingEdges()
    {
        for (int i = 0; i < 1_000_000; i++)
            _graph.TryAddEdge(i, i + 1);
    }

    [Benchmark]
    public bool ContainsEdge()
    {
        return _graph.ContainsEdge(500000, 500001);
    }
}