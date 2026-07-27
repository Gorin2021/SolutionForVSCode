using System.Text;

public class Graph<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _graph;
    private readonly bool _directed;
    private int _edgeCount;

    private readonly IEqualityComparer<T> _comparer;

    public Graph(bool directed = false, IEqualityComparer<T>? comparer = null)
    {
        _directed = directed;
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _graph = new Dictionary<T, HashSet<T>>(_comparer);
    }

    /// <summary>
    /// Получает количество вершин в графе.
    /// </summary>
    public int VertexCount => _graph.Count;

    /// <summary>
    /// Возвращает количество всех рёбер в графе.
    /// 
    /// </summary>
    public int EdgeCount => _edgeCount;

    /// <summary>
    /// Коллекция всех вершин графа.
    /// </summary>  
    public IReadOnlyCollection<T> Vertices => _graph.Keys;

    /// <summary>
    /// Проверяет, существует ли вершина в графе.
    /// </summary>
    /// <param name="vertex">Искомая вершина</param>
    /// <returns>true если вершина существует, false - если не существует</returns>
    public bool ContainsVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        return _graph.ContainsKey(vertex);
    }

    /// <summary>
    /// Проверяет, существует ли ребро между двумя вершинами.
    /// </summary>
    /// <param name="src">Вершина источник</param>
    /// <param name="dest">Вершина цель</param>
    /// <returns>Если ребро найдено, то ворнет true, иначе false</returns>
    public bool ContainsEdge(T src, T dest)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);

        return _graph.TryGetValue(src, out var edges)
        && edges.Contains(dest);
    }

    /// <summary>
    /// Добавляет вершину в граф. Если вершина уже существует, метод не выполняет никаких действий.
    /// </summary>
    /// <param name="vertex">Добавляемая вершина</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если vertex равен null.</exception>
    /// <returns>Возвращает true - добавление произошло успешно; false - такая вершина уже существует в графе.</returns>
    public bool TryAddVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        return _graph.TryAdd(vertex, new HashSet<T>(_comparer));
    }

    /// <summary>
    /// Добавляет ребро между двумя вершинами графа.
    /// Если вершины не существуют, они будут созданы.
    /// Если граф неориентированный, добавляется обратное ребро.
    /// </summary>
    /// <param name="src">Источник ребра</param>
    /// <param name="dest">Назначение ребра</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если src или dest равны null.</exception>
    /// <exception cref="ArgumentException">Выбрасывается, если src или dest равны (петля).</exception>
	public bool TryAddEdge(T src, T dest)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);
        if (_comparer.Equals(src, dest)) throw new ArgumentException("src и dst должны быть разными");

        TryAddVertex(src);
        TryAddVertex(dest);

        bool isAdded = _graph[src].Add(dest);

        if (isAdded) _edgeCount++;

        if (!_directed)
        {
            _graph[dest].Add(src);
        }

        return isAdded;
    }

    /// <summary>
    /// Удаляет вершину из графа.
    /// </summary>
    /// <param name="vertex">Удаляемая вершина</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если vertex равен null.</exception>
    public bool TryRemoveVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (!_graph.TryGetValue(vertex, out var toNeighbours))
            return false;

        if (!_directed)
        {
            foreach (var neighbour in toNeighbours)
            {
                // Для не ориентированного графа удаляем обратные ребра
                // без изменения их количества
                _graph[neighbour].Remove(vertex);
            }
        }
        else
        {
            foreach (var neighbours in _graph.Values)
            {
                if (neighbours.Remove(vertex))
                    _edgeCount--;
            }
        }
        
        _edgeCount -= toNeighbours.Count;

        // Удаляем саму вершину
        return _graph.Remove(vertex);
    }

    /// <summary>
    /// Удаляет ребро между двумя вершинами графа.
    /// Если граф неориентированный, удаляется обратное ребро.
    /// </summary>
    /// <param name="src">Источник ребра</param>
    /// <param name="dest">Назначение ребра</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если src или dest равны null.</exception>
    public bool TryRemoveEdge(T src, T dest)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);

        bool isRemoved = false;

        if (_graph.TryGetValue(src, out var edges))
        {
            isRemoved = edges.Remove(dest);
            if (isRemoved) _edgeCount--;
        }

        if (!_directed && _graph.TryGetValue(dest, out var reverseEdges))
        {
            reverseEdges.Remove(src);
        }
        return isRemoved;
    }

    /// <summary>
    /// Возвращает список смежных вершин для заданной вершины.
    /// </summary>
    /// <param name="vertex">Вершина, для которой нужно получить список смежных вершин</param>
    /// <returns>Список смежных вершин</returns>
    public IReadOnlyCollection<T> GetAdjacentVertices(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (_graph.TryGetValue(vertex, out var edges))
        {
            return edges;
        }

        return Array.Empty<T>();
    }


    /// <summary>
    /// Получение списка всех узлов графа и их смежных вершин в виде строки.
    /// </summary>
    /// <returns>Строка с информацией о графе</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var pair in _graph)
        {
            sb.Append(pair.Key);
            sb.Append(": ");
            sb.AppendJoin(", ", pair.Value);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}