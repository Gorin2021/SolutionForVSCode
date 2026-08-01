using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class AddVertexWithListOfWeightBenchmarks
{
    [Benchmark]
    public GraphWithListOfWeight<int> AddMillionVertices()
    {
        var graph = new GraphWithListOfWeight<int>();

        for (int i = 0; i < 1_000_000; i++)
        {
            graph.TryAddVertex(i);
        }

        return graph;
    }
}