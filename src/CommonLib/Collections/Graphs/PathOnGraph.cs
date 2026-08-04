/// <summary>
/// Представляет путь в графе,  коллекцию типа PathEdge<T>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PathOnGraph<T> where T : notnull
{

    private List<PathEdge<T>> _paths;


    public PathOnGraph()
    {
        _paths = new List<PathEdge<T>>();
    }

    public PathEdge<T> Last => _paths.Last();


    public PathOnGraph<T> Clone() => new PathOnGraph<T>
    {
        _paths = [.. _paths]
    };

    public bool TryGetPathEdge(T vertex, out PathEdge<T> pathEdge)
    {
        if (vertex is null)
            ArgumentNullException.ThrowIfNull(vertex);

        foreach (var edge in _paths)
        {
            if (edge.Target.Equals(vertex))
            {
                pathEdge = edge;
                return true;
            }
        }

        pathEdge = default;
        return false;
    }

    public int Count => _paths.Count;

    /// <summary>
    ///Добавляет начальное ребро в путь, так как путь начинается с этой вершины сбрасываем вес на 0.
    /// </summary>
    /// <param name="vertex">Начальная вершина</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddStartPathEdge(T vertex)
    {
        if (vertex is null)
            ArgumentNullException.ThrowIfNull(vertex);

        _paths.Add(new PathEdge<T>(vertex, 0));
    }

    /// <summary>
    /// Добавляет вершину в путь с указанным весом.
    /// </summary>
    /// <param name="vertex">Ребро, представленое целевой вершиной.
    /// </param>
    /// <param name="weight">Вес вершины от начала пути. 
    /// Сумируются все предыдущие веса ребер</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Add(Edge<T> edge)
    {
        if (edge.Target is null)
            ArgumentNullException.ThrowIfNull(edge.Target);

        if (double.IsNaN(edge.Weight) || double.IsInfinity(edge.Weight))
            throw new ArgumentOutOfRangeException(nameof(edge.Weight));

        if (_paths.Count == 0)
        {
            _paths.Add(new PathEdge<T>(edge.Target, edge.Weight));
        }
        else
        {
            _paths.Add(new PathEdge<T>(edge.Target, edge.Weight +
            ((double)_paths.Last().PathWeight)));
        }
    }
}