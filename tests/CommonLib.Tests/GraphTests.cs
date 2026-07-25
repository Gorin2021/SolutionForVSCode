[TestClass]
public class GraphTests
{
    /// <summary>
    /// Проверяет, что добавление новой вершины увеличивает количество вершин в графе.
    /// </summary>
    [TestMethod]
    public void TryAddVertex_NewVertex_ShouldIncreaseVertexCount()
    {
        // Arrange
        var graph = new Graph<int>();

        // Action
        var added = graph.TryAddVertex(1);

        // Assert
        Assert.IsTrue(added);
        Assert.AreEqual(1, graph.VertexCount);
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.Vertices.ToArray());
    }

    /// <summary>
    /// Проверяет, что свойство EdgeCount возвращает общее количество рёбер в неориентированном графе.
    /// </summary>
    [TestMethod]
    public void EdgeCount_ShouldReturnTotalNumberOfEdges_OnUnDirectedGraf()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);
        graph.TryAddEdge(2, 3);

        // Assert
        Assert.AreEqual(2, graph.EdgeCount);
    }

    /// <summary>
    /// Проверяет, что свойство EdgeCount возвращает общее количество рёбер в неориентированном графе.
    /// </summary>
    [TestMethod]
    public void EdgeCount_ShouldReturnTotalNumberOfEdges_OnDirerctedGraf()
    {
        // Arrange
        var graph = new Graph<int>(true);
        graph.TryAddEdge(1, 2);
        graph.TryAddEdge(2, 3);

        // Assert
        Assert.AreEqual(2, graph.EdgeCount);
    }

    [TestMethod]
    public void EdgeCount_ShouldReturnTotalOfEdges_WhenRemoved()
    {
        // Arrange
        var graph = new Graph<int>(true);

        graph.TryAddEdge(1, 2);
        Assert.AreEqual(1, graph.EdgeCount);

        bool isRemoved = graph.TryRemoveEdge(1, 2);

        // Action
        int result = graph.EdgeCount;

        // Assert
        Assert.IsTrue(isRemoved);
        Assert.AreEqual(0, result);
    }

/// <summary>
/// При удалении вершины, удаляются ее ребра, счетчик корректно уменьшается.
/// проверяет как ориентированный, так и не ориентированный граф 
/// </summary>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void EdgeCount_ShouldReturnTotalEdges_WhenVertexRemoved(bool isDirected)
    {

         // Arrange
        var graph = new Graph<int>(isDirected);

        graph.TryAddEdge(1, 2);
        graph.TryAddEdge(2, 3);
        
        bool isRemoved = graph.TryRemoveVertex(2);

        //Assert
         Assert.IsTrue(isRemoved);
         Assert.AreEqual(0, graph.EdgeCount);

    }

    [TestMethod]
    public void ContainsEdge_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        var graph = new Graph<int>(true);

        // Action
        bool result = graph.ContainsEdge(1, 2);

        //Assert
        Assert.IsFalse(result);
    }



    /// <summary>
    /// Проверяет, что повторное добавление той же вершины не изменяет количество вершин.
    /// </summary>
    [TestMethod]
    public void TryAddVertex_DuplicateVertex_ShouldReturnFalseAndNotChangeCount()
    {
        // Arrange
        var graph = new Graph<int>();

        // Action
        var firstAdd = graph.TryAddVertex(1);
        var secondAdd = graph.TryAddVertex(1);

        // Assert
        Assert.IsTrue(firstAdd);
        Assert.IsFalse(secondAdd);
        Assert.AreEqual(1, graph.VertexCount);
    }

    /// <summary>
    /// Проверяет, что в неориентированном графе ребро добавляет двустороннюю связь между вершинами.
    /// </summary>
    [TestMethod]
    public void TryAddEdge_UndirectedGraph_ShouldCreateVerticesAndAddBidirectionalConnection()
    {
        // Arrange
        var graph = new Graph<int>();

        // Action
        bool result = graph.TryAddEdge(1, 2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(2, graph.VertexCount);
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetAdjacentVertices(1).ToArray());
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetAdjacentVertices(2).ToArray());
    }

    /// <summary>
    /// Проверяет, что в ориентированном графе ребро добавляет только исходящую связь.
    /// </summary>
    [TestMethod]
    public void TryAddEdge_DirectedGraph_ShouldAddOnlyOutgoingConnection()
    {
        // Arrange
        var graph = new Graph<int>(directed: true);

        // Action
        bool result = graph.TryAddEdge(1, 2);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(2, graph.VertexCount);
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetAdjacentVertices(1).ToArray());
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(2).ToArray());
    }

    /// <summary>
    /// Проверяет, что удаление ребра в неориентированном графе убирает связь в обе стороны.
    /// </summary>
    [TestMethod]
    public void TryRemoveEdge_UndirectedGraph_ShouldRemoveBothDirections()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);

        // Action
        bool result = graph.TryRemoveEdge(1, 2);

        // Assert
        Assert.IsTrue(result);
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(1).ToArray());
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(2).ToArray());
    }

    [TestMethod]
    public void TryRemoveEdge_EdgeCountNotChanged_WhenEdgeNotExists()
    {
        //Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);
        Assert.AreEqual(1, graph.EdgeCount);

        //Action
        var isRemoved = graph.TryRemoveEdge(1, 3);

        //Assert
        Assert.IsFalse(isRemoved);
        Assert.AreEqual(1, graph.EdgeCount);
    }

    /// <summary>
    /// Проверяет, что удаление вершины очищает связанные с ней ребра у соседних вершин.
    /// </summary>
    [TestMethod]
    public void TryRemoveVertex_ShouldRemoveVertexAndCleanIncomingEdges()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);
        graph.TryAddEdge(2, 3);

        // Action
        bool result = graph.TryRemoveVertex(2);

        // Assert
        Assert.AreEqual(2, graph.VertexCount);
        Assert.IsTrue(result);
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(1).ToArray());
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(3).ToArray());
    }

    [TestMethod]
    public void TryRemoveVertex_DirectedGraph_ShouldRemoveVertexAndCleanIncomingEdges()
    {
        // Arrange
        var graph = new Graph<int>(directed: true);
        graph.TryAddEdge(1, 2);
        graph.TryAddEdge(2, 3);

        // Action
        bool result = graph.TryRemoveVertex(2);

        // Assert
        Assert.AreEqual(2, graph.VertexCount);
        Assert.IsTrue(result);
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(1).ToArray());
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(3).ToArray());
    }

    /// <summary>
    /// Проверяет, что повторное добавление уже существующего ребра возвращает false.
    /// </summary>
    [TestMethod]
    public void TryAddEdge_DuplicateEdge_ShouldReturnFalse()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);

        // Action
        bool result = graph.TryAddEdge(1, 2);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Проверяет, что удаление несуществующего ребра возвращает false.
    /// </summary>
    [TestMethod]
    public void TryRemoveEdge_MissingEdge_ShouldReturnFalse()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddVertex(1);
        graph.TryAddVertex(2);

        // Action
        bool result = graph.TryRemoveEdge(1, 2);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Проверяет, что удаление несуществующей вершины возвращает false и не меняет состояние графа.
    /// </summary>
    [TestMethod]
    public void TryRemoveVertex_MissingVertex_ShouldReturnFalse()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddEdge(1, 2);

        // Action
        bool result = graph.TryRemoveVertex(3);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(2, graph.VertexCount);
    }

    /// <summary>
    /// Проверяет, что запрос смежных вершин для отсутствующей вершины возвращает пустую коллекцию.
    /// </summary>
    [TestMethod]
    public void GetAdjacentVertices_MissingVertex_ShouldReturnEmptyCollection()
    {
        // Arrange
        var graph = new Graph<int>();
        graph.TryAddVertex(1);

        // Assert
        CollectionAssert.AreEquivalent(Array.Empty<int>(), graph.GetAdjacentVertices(2).ToArray());
    }

    /// <summary>
    /// Проверяет, что строковое представление графа корректно отображает вершины и их соседей.
    /// </summary>
    [TestMethod]
    public void ToString_ShouldRenderVerticesAndTheirNeighbors()
    {
        // Arrange
        var graph = new Graph<string>();
        graph.TryAddEdge("A", "B");

        // Action
        var text = graph.ToString();

        // Assert
        StringAssert.Contains(text, "A: B");
        StringAssert.Contains(text, "B: A");
    }
}
