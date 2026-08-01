using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class EdgeWithWeightBenchmark
{
   
    private GraphWithWeight<int> _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        _graph = new GraphWithWeight<int>();

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
        [Benchmark]
    public bool ContainsVertex()
    {
        return _graph.ContainsVertex(10_000_002);
    }
}