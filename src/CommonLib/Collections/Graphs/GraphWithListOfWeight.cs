using System.Text;
using GraphLibrary;

/// <summary>
/// Представляет граф с оптимизированной структурой для частых операций добавления/удаления.
/// Использует List&lt;Edge&lt;T&gt;&gt; для хранения смежных вершин, что обеспечивает 
/// лучшую локальность данных и меньшее давление на GC.
/// </summary>
/// <typeparam name="T">Тип вершин графа. Должен быть не nullable.</typeparam>
public class GraphWithListOfWeight<T> where T : notnull
    {
        private readonly Dictionary<T, List<Edge<T>>> _graph;
        private readonly bool _directed;
        private readonly IEqualityComparer<T> _comparer;
        private int _edgeCount;

        /// <summary>
        /// Инициализирует новый экземпляр класса FastGraph.
        /// </summary>
        /// <param name="directed">
        /// Если true, создаётся ориентированный граф; 
        /// если false — неориентированный.
        /// </param>
        /// <param name="comparer">
        /// Сравнитель для вершин. Если null, используется EqualityComparer&lt;T&gt;.Default.
        /// </param>
        public GraphWithListOfWeight(bool directed = false, IEqualityComparer<T>? comparer = null)
        {
            _directed = directed;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _graph = new Dictionary<T, List<Edge<T>>>(_comparer);
            _edgeCount = 0;
        }

        /// <summary>
        /// Получает количество вершин в графе.
        /// </summary>
        public int VertexCount => _graph.Count;

        /// <summary>
        /// Получает количество рёбер в графе.
        /// Для неориентированного графа каждое ребро считается один раз.
        /// </summary>
        public int EdgeCount => _edgeCount;

        /// <summary>
        /// Получает коллекцию всех вершин графа.
        /// </summary>
        public IReadOnlyCollection<T> Vertices => _graph.Keys;

        /// <summary>
        /// Проверяет, существует ли указанная вершина в графе.
        /// </summary>
        /// <param name="vertex">Вершина для проверки.</param>
        /// <returns>
        /// true, если вершина существует в графе; 
        /// иначе false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если vertex равен null.
        /// </exception>
        public bool ContainsVertex(T vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);
            return _graph.ContainsKey(vertex);
        }

        /// <summary>
        /// Проверяет, существует ли ребро между двумя вершинами.
        /// </summary>
        /// <param name="src">Исходная вершина.</param>
        /// <param name="dest">Целевая вершина.</param>
        /// <returns>
        /// true, если ребро существует; иначе false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если src или dest равны null.
        /// </exception>
        public bool ContainsEdge(T src, T dest)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);

            if (!_graph.TryGetValue(src, out var edges))
                return false;

            return FindEdgeIndex(edges, dest) >= 0;
        }

        /// <summary>
        /// Пытается получить вес ребра между двумя вершинами.
        /// </summary>
        /// <param name="src">Исходная вершина.</param>
        /// <param name="dest">Целевая вершина.</param>
        /// <param name="weight">
        /// Вес ребра, если оно существует; иначе 0.
        /// </param>
        /// <returns>
        /// true, если ребро найдено; иначе false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если src или dest равны null.
        /// </exception>
        public bool TryGetEdgeWeight(T src, T dest, out double weight)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);
            weight = 0.0;

            if (!_graph.TryGetValue(src, out var edges))
                return false;

            var index = FindEdgeIndex(edges, dest);
            if (index >= 0)
            {
                weight = edges[index].Weight;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Добавляет вершину в граф.
        /// Если вершина уже существует, метод не выполняет никаких действий.
        /// </summary>
        /// <param name="vertex">Вершина для добавления.</param>
        /// <returns>
        /// true, если вершина была добавлена; 
        /// false, если вершина уже существует.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если vertex равен null.
        /// </exception>
        public bool TryAddVertex(T vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);
            return _graph.TryAdd(vertex, new List<Edge<T>>());
        }

        /// <summary>
        /// Добавляет ребро между двумя вершинами с указанным весом.
        /// Если вершины не существуют, они будут автоматически созданы.
        /// Если ребро уже существует, его вес будет обновлён.
        /// Для неориентированного графа добавляется/обновляется обратное ребро.
        /// </summary>
        /// <param name="src">Исходная вершина.</param>
        /// <param name="dest">Целевая вершина.</param>
        /// <param name="weight">Вес ребра (по умолчанию 1.0).</param>
        /// <returns>
        /// true, если было добавлено новое ребро; 
        /// false, если ребро уже существовало и было обновлено.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если src или dest равны null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если src и dest указывают на одну и ту же вершину (петли не поддерживаются).
        /// </exception>
        public bool TryAddEdge(T src, T dest, double weight = 1.0)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);

            if (_comparer.Equals(src, dest))
                throw new ArgumentException("Петли не поддерживаются: src и dest должны быть разными вершинами", nameof(dest));

            // Гарантируем существование вершин
            TryAddVertex(src);
            TryAddVertex(dest);

            var srcEdges = _graph[src];

            // Проверяем, существует ли уже ребро
            var existingIndex = FindEdgeIndex(srcEdges, dest);
            
            if (existingIndex >= 0)
            {
                // Ребро существует - обновляем вес
                srcEdges[existingIndex] = new Edge<T>(dest, weight);

                if (!_directed)
                {
                    // Обновляем обратное ребро
                    var destEdges = _graph[dest];
                    var reverseIndex = FindEdgeIndex(destEdges, src);
                    if (reverseIndex >= 0)
                    {
                        destEdges[reverseIndex] = new Edge<T>(src, weight);
                    }
                }

                return false; // Не добавлено, а обновлено
            }

            // Добавляем новое ребро
            srcEdges.Add(new Edge<T>(dest, weight));
            _edgeCount++;

            if (!_directed)
            {
                // Добавляем обратное ребро для неориентированного графа
                _graph[dest].Add(new Edge<T>(src, weight));
            }

            return true;
        }

        /// <summary>
        /// Удаляет вершину из графа вместе со всеми инцидентными рёбрами.
        /// </summary>
        /// <param name="vertex">Вершина для удаления.</param>
        /// <returns>
        /// true, если вершина была найдена и удалена; 
        /// иначе false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если vertex равен null.
        /// </exception>
        public bool TryRemoveVertex(T vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            if (!_graph.TryGetValue(vertex, out var edgesToRemove))
                return false;

            // Для неориентированного графа: удаляем ссылки на эту вершину из смежных вершин
            if (!_directed)
            {
                foreach (var edge in edgesToRemove)
                {
                    if (_graph.TryGetValue(edge.Target, out var adjacentEdges))
                    {
                        RemoveEdgeSwapAndPop(adjacentEdges, vertex);
                    }
                }
            }
            else
            {
                // Для ориентированного графа: ищем и удаляем входящие рёбра
                foreach (var edges in _graph.Values)
                {
                    var index = FindEdgeIndex(edges, vertex);
                    if (index >= 0)
                    {
                        RemoveEdgeSwapAndPop(edges, vertex);
                        _edgeCount--;
                    }
                }
            }

            // Вычитаем количество исходящих рёбер из общего счётчика
            _edgeCount -= edgesToRemove.Count;

            // Удаляем саму вершину
            return _graph.Remove(vertex);
        }

        /// <summary>
        /// Удаляет ребро между двумя вершинами.
        /// Для неориентированного графа удаляется также обратное ребро.
        /// </summary>
        /// <param name="src">Исходная вершина.</param>
        /// <param name="dest">Целевая вершина.</param>
        /// <returns>
        /// true, если ребро было найдено и удалено; 
        /// иначе false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если src или dest равны null.
        /// </exception>
        public bool TryRemoveEdge(T src, T dest)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);

            if (!_graph.TryGetValue(src, out var srcEdges))
                return false;

            bool removed = RemoveEdgeSwapAndPop(srcEdges, dest);
            
            if (removed)
            {
                _edgeCount--;

                if (!_directed)
                {
                    // Удаляем обратное ребро для неориентированного графа
                    if (_graph.TryGetValue(dest, out var destEdges))
                    {
                        RemoveEdgeSwapAndPop(destEdges, src);
                    }
                }
            }

            return removed;
        }

        /// <summary>
        /// Получает коллекцию смежных вершин и весов рёбер для указанной вершины.
        /// </summary>
        /// <param name="vertex">Вершина, для которой нужно получить смежные вершины.</param>
        /// <returns>
        /// Коллекция рёбер (целевая вершина + вес). 
        /// Если вершина не существует, возвращается пустая коллекция.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если vertex равен null.
        /// </exception>
        /// <remarks>
        /// Возвращается ссылка на внутренний список для производительности. 
        /// Не модифицируйте возвращаемую коллекцию напрямую.
        /// </remarks>
        public IReadOnlyList<Edge<T>> GetAdjacentVertices(T vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            if (_graph.TryGetValue(vertex, out var edges))
            {
                return edges;
            }

            return Array.Empty<Edge<T>>();
        }

        /// <summary>
        /// Очищает граф, удаляя все вершины и рёбра.
        /// </summary>
        public void Clear()
        {
            _graph.Clear();
            _edgeCount = 0;
        }

        /// <summary>
        /// Возвращает строковое представление графа.
        /// </summary>
        /// <returns>
        /// Строка, содержащая информацию о всех вершинах и их смежных вершинах с весами.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Graph ({(_directed ? "directed" : "undirected")}): {_graph.Count} vertices, {_edgeCount} edges");

            foreach (var pair in _graph)
            {
                sb.Append($"  {pair.Key}: ");
                
                if (pair.Value.Count == 0)
                {
                    sb.AppendLine("<no edges>");
                }
                else
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        var edge = pair.Value[i];
                        sb.Append($"{edge.Target}(w:{edge.Weight})");
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        #region Private Helper Methods

        /// <summary>
        /// Находит индекс ребра с указанной целевой вершиной в списке.
        /// </summary>
        /// <param name="edges">Список рёбер для поиска.</param>
        /// <param name="target">Целевая вершина.</param>
        /// <returns>
        /// Индекс ребра, если найдено; иначе -1.
        /// </returns>
        private int FindEdgeIndex(List<Edge<T>> edges, T target)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (_comparer.Equals(edges[i].Target, target))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Удаляет ребро с указанной целевой вершиной из списка, 
        /// используя алгоритм Swap-and-Pop для O(1) удаления.
        /// </summary>
        /// <param name="edges">Список рёбер.</param>
        /// <param name="target">Целевая вершина удаляемого ребра.</param>
        /// <returns>
        /// true, если ребро было найдено и удалено; иначе false.
        /// </returns>
        private bool RemoveEdgeSwapAndPop(List<Edge<T>> edges, T target)
        {
            var index = FindEdgeIndex(edges, target);
            
            if (index < 0)
                return false;

            // Swap-and-Pop: меняем удаляемый элемент с последним
            edges[index] = edges[edges.Count - 1];
            edges.RemoveAt(edges.Count - 1);
            
            return true;
        }

        #endregion
    }