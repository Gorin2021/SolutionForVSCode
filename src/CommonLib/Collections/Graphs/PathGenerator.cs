using System.Diagnostics;

public class PathGenerator<T> where T : notnull
{
    public T StartVertex { get; init; }
    public T EndVertex { get; init; }
    public GraphWithListOfWeight<T> Graph { get; init; }
    private Paths<T> _paths = new Paths<T>();

    public Paths<T>? GeneratedPath { get; private set; }
    private IEqualityComparer<T> Comparer { get; }

    public PathGenerator(T startVertex, T endVertex,
     GraphWithListOfWeight<T> graph, IEqualityComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(endVertex);
        ArgumentNullException.ThrowIfNull(graph);
        StartVertex = startVertex;
        EndVertex = endVertex;
        Comparer = comparer ?? EqualityComparer<T>.Default;
        Graph = graph;
    }

    public void GeneratePaths()
    {
        //Добавляем начальную вершину в путь с весом 0, так как путь начинается с этой вершины.
        _paths.Add(new Edge<T>(StartVertex, 0));
        GeneratePath(StartVertex);
        Debug.WriteLine($"Результат - путь с количеством вершин: { GeneratedPath?.Count ?? 0},  с весом: {GeneratedPath?.Last.PathWeight ?? 0}");
    }

    private void GeneratePath(T vertex, T? prefVertex = default)
    {
        foreach (var edge in Graph.GetAdjacentVertices(vertex))
        {
            double pathWeight = _paths.Last.PathWeight + edge.Weight;

            if (Comparer.Equals(edge.Target, StartVertex)
            || Comparer.Equals(edge.Target, prefVertex)
            || GeneratedPath is not null && pathWeight >= GeneratedPath.Last.PathWeight)
                continue;

            if (_paths.TryGetPathEdge(edge.Target, out var existingPathEdge))
            {
                if (existingPathEdge.PathWeight > 0 
                && existingPathEdge.PathWeight <= pathWeight)
                    continue;
            }

            if (Comparer.Equals(edge.Target, EndVertex))
            {
                if (GeneratedPath is null
                || pathWeight < GeneratedPath.Last.PathWeight)

                    GeneratedPath = _paths.Clone();

                GeneratedPath.Add(edge);

                Debug.WriteLine($"Найден путь с количеством вершин: { GeneratedPath.Count},  с весом: {GeneratedPath.Last.PathWeight}");
                
                continue;
            }

            _paths.Add(edge);

            GeneratePath(edge.Target, vertex);
        }
    }
}