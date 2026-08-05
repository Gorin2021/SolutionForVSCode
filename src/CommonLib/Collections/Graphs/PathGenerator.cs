using System.Diagnostics;

/// <summary>
/// Представляет генератор оптимального пути в графе с весами.
/// </summary>
/// <typeparam name="T">Тип вершины</typeparam>
public class PathGenerator<T> where T : notnull
{
    /// <summary>
    /// Начальная вершина пути, с которой начинается генерация оптимального пути.
    /// </summary>
    public T StartVertex { get; private set; }

    /// <summary>
    /// Конечная вершина пути, до которой генерируется оптимальный путь.
    /// </summary>
    public T EndVertex { get; private set; }

    /// <summary>
    /// Граф, в котором осуществляется генерация оптимального пути.
    /// </summary>
    public GraphWithListOfWeight<T> Graph { get; init; }

    /// <summary>
    /// Представляет текущий путь, который генерируется в процессе поиска оптимального пути.
    /// </summary>
    private PathOnGraph<T> _paths = new PathOnGraph<T>();

    /// <summary>
    /// Представляет сгенерированный оптимальный путь между начальной и конечной вершинами.
    /// Если путь не найден, значение будет null.
    /// </summary>
    public PathOnGraph<T>? GeneratedPath { get; private set; }

    /// <summary>
    /// Представляет компаратор для сравнения вершин типа T, используемый при генерации пути.
    /// Если компаратор не предоставлен, используется стандартный компаратор по умолчанию.
    /// </summary>
    private IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса PathGenerator с указанным графом и компаратором вершин.
    /// </summary>
    /// <param name="graph">Граф, в котором осуществляется генерация оптимального пути</param>
    /// <param name="comparer">Компаратор для сравнения вершин типа T</param>
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

    /// <summary>
    /// Генерирует оптимальный путь между двумя вершинами в графе.
    /// </summary>
    /// <param name="startVertex">Начальная вершина</param>
    /// <param name="endVertex">Конечная вершина</param>
    /// <exception cref="ArgumentNullException">Выбрасывается если startVertex или endVertex равно null</exception>
    /// <exception cref="ArgumentException">Выбрасывается если startVertex или endVertex:
    ///  не существует в графе или не содержит ребер (только при не ориентированом графе).</exception> 
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
    /// <exception cref="ArgumentException">Выбрасывается если vertex
    ///  не существует в графе или не содержит ребер (только при не ориентированом графе).</exception>
    private void ValidateStartAndEndVertexes(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (!Graph.ContainsVertex(vertex!))
            throw new ArgumentException($"Граф не содержит вершину {vertex}");

        if (!Graph.IsDirected)
        {
            if (!Graph.GetAdjacentVertices(vertex).Any())
                throw new ArgumentException($"Вершина не содержет ребер {vertex}");
        }
    }
    /// <summary>
    /// Рекурсивно генерирует оптимальный путь от текущей вершины к конечной вершине, обходя все смежные вершины.
    /// Если найден путь к конечной вершине, обновляет GeneratedPath, если найденный путь имеет меньший вес, чем уже существующий путь.
    /// </summary>
    /// <param name="vertex">Текущая вершина</param>
    /// <param name="prefVertex">Предыдущая вершина. 
    /// Если граф не ориентированный, используется для
    /// предотвращения возврата к предыдущей вершине</param>
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

                GeneratedPath.AddPathEdge(edge);

                Debug.WriteLine($"Найден путь с количеством вершин: {GeneratedPath.Count},  с весом: {GeneratedPath.Last.PathWeight}");

                continue;
            }

            _paths.AddPathEdge(edge);

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