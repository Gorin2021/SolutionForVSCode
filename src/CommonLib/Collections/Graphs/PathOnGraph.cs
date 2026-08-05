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

    /// <summary>
    /// Возвращает последнее ребро пути,
    ///  которое содержит целевую вершину и накопленный вес пути.
    /// </summary>
    public PathEdge<T> Last => _paths.Last();

#if DEBUG

    public int Count => _paths.Count;

#endif

    /// <summary>
    /// Создает клон текущего пути, создавая новый экземпляр PathOnGraph<T> с копией всех ребер пути.
    /// </summary>
    /// <returns>Клон текущего пути</returns>
    public PathOnGraph<T> Clone() => new PathOnGraph<T>
    {
        _paths = [.. _paths]
    };

/// <summary>
/// Пытается получить ребро пути по целевой вершине.
/// Если вершина найдена, возвращает true и присваивает найденное ребро переменной pathEdge,
/// иначе возвращает false и присваивает pathEdge значение по умолчанию.
/// </summary>
/// <param name="vertex">Целевая вершина</param>
/// <param name="pathEdge">Переменная для хранения найденного ребра</param>
/// <returns>Если вершина найдена, возвращает true, иначе false</returns>
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
    /// Добавляет ребро в путь с указанным весом.
    /// </summary>
    /// <param name="edge">Ребро, которое нужно добавить в путь</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddPathEdge(Edge<T> edge)
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