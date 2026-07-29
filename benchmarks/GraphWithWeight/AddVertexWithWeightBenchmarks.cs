using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class AddVertexWithWeightBenchmarks
{
    [Benchmark]
    public GraphWithWeight<int> AddMillionVertices()
    {
        var graph = new GraphWithWeight<int>();

        for (int i = 0; i < 1_000_000; i++)
        {
            graph.TryAddVertex(i);
        }

        return graph;
    }
}