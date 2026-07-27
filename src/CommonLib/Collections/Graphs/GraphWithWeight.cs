using System.Text;

public class GraphWithWeight<T> where T : notnull
{
    // Dictionary<T, double> хранит смежные вершины (ключ) и вес ребра (значение)
    private readonly Dictionary<T, Dictionary<T, double>> _graph;
    private readonly bool _directed;
    private int _edgeCount;
    private readonly IEqualityComparer<T> _comparer;

    public GraphWithWeight(bool directed = false, IEqualityComparer<T>? comparer = null)
    {
        _directed = directed;
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _graph = new Dictionary<T, Dictionary<T, double>>(_comparer);
    }

    /// <summary>
    /// Получает количество вершин в графе.
    /// </summary>
    public int VertexCount => _graph.Count;

    /// <summary>
    /// Возвращает количество всех рёбер в графе.
    /// </summary>
    public int EdgeCount => _edgeCount;

    /// <summary>
    /// Коллекция всех вершин графа.
    /// </summary>  
    public IReadOnlyCollection<T> Vertices => _graph.Keys;

    /// <summary>
    /// Проверяет, существует ли вершина в графе.
    /// </summary>
    public bool ContainsVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);
        return _graph.ContainsKey(vertex);
    }

    /// <summary>
    /// Проверяет, существует ли ребро между двумя вершинами.
    /// </summary>
    public bool ContainsEdge(T src, T dest)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);
        return _graph.TryGetValue(src, out var edges) && edges.ContainsKey(dest);
    }

    /// <summary>
    /// Пытается получить вес ребра между двумя вершинами.
    /// </summary>
    /// <param name="src">Вершина источник</param>
    /// <param name="dest">Вершина цель</param>
    /// <param name="weight">Вес ребра, если оно существует</param>
    /// <returns>true если ребро найдено, иначе false</returns>
    public bool TryGetEdgeWeight(T src, T dest, out double weight)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);
        
        weight = 0.0;
        return _graph.TryGetValue(src, out var edges) && edges.TryGetValue(dest, out weight);
    }

    /// <summary>
    /// Добавляет вершину в граф. Если вершина уже существует, метод не выполняет никаких действий.
    /// </summary>
    public bool TryAddVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);
        return _graph.TryAdd(vertex, new Dictionary<T, double>(_comparer));
    }

    /// <summary>
    /// Добавляет ребро между двумя вершинами графа с указанным весом.
    /// Если вершины не существуют, они будут созданы.
    /// Если граф неориентированный, добавляется или обновляется обратное ребро с тем же весом.
    /// Если ребро уже существует, его вес будет обновлен, но метод вернет false (так как новое ребро не добавлялось).
    /// </summary>
    public bool TryAddEdge(T src, T dest, double weight = 1.0)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(dest);
        
        if (_comparer.Equals(src, dest)) 
            throw new ArgumentException("src и dest должны быть разными вершинами (петли не поддерживаются)");

        TryAddVertex(src);
        TryAddVertex(dest);

        bool isAdded = !_graph[src].ContainsKey(dest);
        
        // Добавляем или обновляем вес ребра
        _graph[src][dest] = weight;
        
        if (isAdded)
        {
            _edgeCount++;
            if (!_directed)
            {
                _graph[dest][src] = weight;
            }
        }
        else if (!_directed)
        {
            // Если ребро уже было, но вес изменился, синхронизируем обратное ребро
            _graph[dest][src] = weight;
        }

        return isAdded;
    }

    /// <summary>
    /// Удаляет вершину из графа и все связанные с ней рёбра.
    /// </summary>
    public bool TryRemoveVertex(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (!_graph.TryGetValue(vertex, out var toNeighbours))
            return false;

        if (!_directed)
        {
            // Для неориентированного графа удаляем обратные ссылки без изменения счетчика рёбер
            // (так как эти рёбра уже будут учтены при вычитании toNeighbours.Count)
            foreach (var neighbour in toNeighbours.Keys)
            {
                _graph[neighbour].Remove(vertex);
            }
        }
        else
        {
            // Для ориентированного графа ищем вершину в списках смежности других вершин
            foreach (var neighbours in _graph.Values)
            {
                if (neighbours.Remove(vertex))
                    _edgeCount--;
            }
        }
        
        _edgeCount -= toNeighbours.Count;
        return _graph.Remove(vertex);
    }

    /// <summary>
    /// Удаляет ребро между двумя вершинами графа.
    /// Если граф неориентированный, удаляется и обратное ребро.
    /// </summary>
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
    /// Возвращает словарь смежных вершин и весов рёбер для заданной вершины.
    /// </summary>
    /// <remarks>
    /// Возвращается ссылка на внутренний словарь для избежания лишних аллокаций памяти. 
    /// Не модифицируйте возвращаемую коллекцию напрямую, используйте методы TryAddEdge / TryRemoveEdge.
    /// </remarks>
    public IReadOnlyDictionary<T, double> GetAdjacentVertices(T vertex)
    {
        ArgumentNullException.ThrowIfNull(vertex);

        if (_graph.TryGetValue(vertex, out var edges))
        {
            return edges;
        }

        return new Dictionary<T, double>(_comparer);
    }

    /// <summary>
    /// Получение списка всех узлов графа и их смежных вершин с весами в виде строки.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var pair in _graph)
        {
            sb.Append(pair.Key).Append(": ");
            
            bool first = true;
            foreach (var edge in pair.Value)
            {
                if (!first) sb.Append(", ");
                sb.Append(edge.Key).Append("(w:").Append(edge.Value).Append(")");
                first = false;
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}