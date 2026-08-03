public struct PathEdge<T>
{
    public T Target { get; init; }
    public double PathWeight { get; set; }
    public PathEdge(T target, double pathWeight)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (double.IsNaN(pathWeight))
            throw new ArgumentOutOfRangeException(nameof(pathWeight));

        if (double.IsNegativeInfinity(pathWeight))
            throw new ArgumentOutOfRangeException(nameof(pathWeight));

        Target = target;
        PathWeight = pathWeight;
    }
}

/// <summary>
/// Представляет путь в графе,  коллекцию типа PathEdge<T>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Paths<T> where T : notnull
{
    
    private List<PathEdge<T>> _paths;


    public Paths()
    {
        _paths = new List<PathEdge<T>>();
    }

    public Paths<T> Clone() => new Paths<T>
    {
        _paths = [.. _paths]
    };

    public bool TryGetPathEdge(T vertex, out PathEdge<T> pathEdge)
    {
        if (vertex is null)
            throw new ArgumentNullException(nameof(vertex));

        if (_paths.FirstOrDefault(e => e.Target.Equals(vertex)) is PathEdge<T> foundPathEdge)
        {
            pathEdge = foundPathEdge;
            return true;
        }

        pathEdge = default;
        return false;
    }

    public int Count => _paths.Count;

    /// <summary>
    /// Добавляет вершину в путь с указанным весом.
    /// </summary>
    /// <param name="vertex">Ребро, представленое целевой вершиной.
    /// </param>
    /// <param name="weight">Вес вершины от начала пути. 
    /// Сумируются все предыдущие веса ребер </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Add(Edge<T> edge)
    {
        if(edge.Target is null)
            throw new ArgumentNullException(nameof(edge.Target));
        if (double.IsNaN(edge.Weight))
            throw new ArgumentOutOfRangeException(nameof(edge.Weight));

        if (double.IsNegativeInfinity(edge.Weight))
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
    
    public bool TryGetWeight(T vertex, out double weight)
    {
        if (_paths.FirstOrDefault(e => e.Target.Equals(vertex)) is PathEdge<T> pathEdge)
        {
            weight = pathEdge.PathWeight;
            return true;
        }

        weight = 0;
        return false;
    }

    public PathEdge<T> Last => _paths.Last();
}