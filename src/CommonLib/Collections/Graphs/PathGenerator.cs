using System.Diagnostics;

public class PathGenerator<T> where T : notnull
{
    public T StartVertex { get; private set; }
    public T EndVertex { get; private set; }
    public GraphWithListOfWeight<T> Graph { get; init; }
    private PathOnGraph<T> _paths = new PathOnGraph<T>();

    public PathOnGraph<T>? GeneratedPath { get; private set; }
    private IEqualityComparer<T> Comparer { get; }

    public PathGenerator(GraphWithListOfWeight<T> graph, IEqualityComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(graph);

        Graph = graph;

        Comparer = comparer ?? EqualityComparer<T>.Default;

        // Убираем предупреждение о том что StartVertex и EndVertex не инициализированы,
        // так как они будут инициализированы в методе GeneratePaths.
        StartVertex = default!;
        EndVertex = default!;
    }

    public void GeneratePaths(T startVertex, T endVertex)
    {
        ValidateStartAndEndVertexes(startVertex);
        ValidateStartAndEndVertexes(endVertex);


        StartVertex = startVertex;
        EndVertex = endVertex;

        _paths.AddStartPathEdge(StartVertex);

        GenerateOptimalPath(StartVertex);

        Debug.WriteLine($"Результат - путь с количеством вершин: {GeneratedPath?.Count ?? 0},  с весом: {GeneratedPath?.Last.PathWeight ?? 0}");
    }

    /// <summary>
    /// Проверяет входящие значения вершин на null, а также проверяет существуют ли вершина в графе и содержат ли она ребра (только при не ориентированом графе).
    /// </summary>
    /// <param name="vertex">Проверяемая вершина</param>
    /// <exception cref="ArgumentNullException">Выбрасывается если vertex равно null
    /// <exception cref="ArgumentException">Выбрасывается если vertex:
    ///  не существует в графе или не содержит ребер (только при не ориентированом графе).</exception>
    private void ValidateStartAndEndVertexes(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (!Graph.ContainsVertex(vertex!))
            throw new ArgumentException($"Граф не содержит вершину {vertex}");

        if (!Graph.IsDirected)
        {
            if (!Graph.GetAdjacentVertices(vertex).Any())
                throw new ArgumentException($"Начальная вершина не содержет ребер {vertex}");
        }
    }

    private void GenerateOptimalPath(T vertex, T? prefVertex = default)
    {
        foreach (var edge in Graph.GetAdjacentVertices(vertex))
        {
            double pathWeight = _paths.Last.PathWeight + edge.Weight;

            if (!IsWeightMoreThanExisting(prefVertex, edge, pathWeight))
                continue;

            if (Comparer.Equals(edge.Target, EndVertex))
            {
                if (GeneratedPath is null
                || pathWeight < GeneratedPath.Last.PathWeight)

                    GeneratedPath = _paths.Clone();

                GeneratedPath.Add(edge);

                Debug.WriteLine($"Найден путь с количеством вершин: {GeneratedPath.Count},  с весом: {GeneratedPath.Last.PathWeight}");

                continue;
            }

            _paths.Add(edge);

            GenerateOptimalPath(edge.Target, vertex);
        }
    }
    /// <summary>
    /// Возвращает false если у ребра Target == StartVertex или Target == prefVertex,
    /// или если вес пути к искомой вершине меньше или равен весу уже существующего пути к этой вершине,
    /// если такой путь уже существует. 
    /// </summary>
    private bool IsWeightMoreThanExisting(T? prefVertex, Edge<T> edge, double pathWeight)
    {
        if (Comparer.Equals(edge.Target, StartVertex)
        || Comparer.Equals(edge.Target, prefVertex)
        || GeneratedPath is not null && pathWeight >= GeneratedPath.Last.PathWeight
        || _paths.TryGetPathEdge(edge.Target, out var path)
        && path.PathWeight > 0d // если вес уже существующего пути к этой вершине больше 0, 
        && path.PathWeight <= pathWeight) //то проверяем вес нового пути
        {
            return false;
        }

        return true;
    }
}