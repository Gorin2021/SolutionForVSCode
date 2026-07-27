using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class AddVertexBenchmarks
{
    [Benchmark]
    public Graph<int> AddMillionVertices()
    {
        var graph = new Graph<int>();

        for (int i = 0; i < 1_000_000; i++)
        {
            graph.TryAddVertex(i);
        }

        return graph;
    }
}