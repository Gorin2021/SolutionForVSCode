using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GraphWithListOfWeightTests
{
    [TestMethod]
    public void TryAddVertex_NewVertex_ShouldIncreaseVertexCount()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();

        // Action
        var added = graph.TryAddVertex(1);

        // Assert
        Assert.IsTrue(added);
        Assert.AreEqual(1, graph.VertexCount);
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.Vertices.ToArray());
    }

    [TestMethod]
    public void TryAddEdge_UndirectedGraph_ShouldCreateBidirectionalEdgesAndIncrementEdgeCount()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>(directed: false);

        // Action
        var added = graph.TryAddEdge(1, 2, 3.5);

        // Assert
        Assert.IsTrue(added);
        Assert.AreEqual(2, graph.VertexCount);
        Assert.AreEqual(1, graph.EdgeCount);
        Assert.IsTrue(graph.ContainsEdge(1, 2));
        Assert.IsTrue(graph.ContainsEdge(2, 1));

        Assert.IsTrue(graph.TryGetEdgeWeight(1, 2, out var weight12));
        Assert.AreEqual(3.5, weight12);

        Assert.IsTrue(graph.TryGetEdgeWeight(2, 1, out var weight21));
        Assert.AreEqual(3.5, weight21);
    }

    [TestMethod]
    public void TryAddEdge_DirectedGraph_ShouldAddOnlyOutgoingEdge()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>(directed: true);

        // Action
        var added = graph.TryAddEdge(1, 2, 2.0);

        // Assert
        Assert.IsTrue(added);
        Assert.AreEqual(2, graph.VertexCount);
        Assert.AreEqual(1, graph.EdgeCount);
        Assert.IsTrue(graph.ContainsEdge(1, 2));
        Assert.IsFalse(graph.ContainsEdge(2, 1));
    }

    [TestMethod]
    public void TryAddEdge_DuplicateEdge_ShouldReturnFalseAndUpdateWeight()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();

        // Action
        var firstAdd = graph.TryAddEdge(1, 2, 1.0);
        var secondAdd = graph.TryAddEdge(1, 2, 4.2);

        // Assert
        Assert.IsTrue(firstAdd);
        Assert.IsFalse(secondAdd);
        Assert.AreEqual(1, graph.EdgeCount);
        Assert.IsTrue(graph.TryGetEdgeWeight(1, 2, out var weight12));
        Assert.AreEqual(4.2, weight12);
        Assert.IsTrue(graph.TryGetEdgeWeight(2, 1, out var weight21));
        Assert.AreEqual(4.2, weight21);
    }

    [TestMethod]
    public void TryRemoveEdge_UndirectedGraph_ShouldRemoveBothDirectionsAndDecreaseEdgeCount()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();
        graph.TryAddEdge(1, 2, 1.0);

        // Action
        var removed = graph.TryRemoveEdge(1, 2);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(0, graph.EdgeCount);
        Assert.IsFalse(graph.ContainsEdge(1, 2));
        Assert.IsFalse(graph.ContainsEdge(2, 1));
    }

    [TestMethod]
    public void TryRemoveVertex_ShouldRemoveVertexAndRelatedEdges()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();
        graph.TryAddEdge(1, 2, 1.0);
        graph.TryAddEdge(2, 3, 2.0);

        // Action
        var removed = graph.TryRemoveVertex(2);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(2, graph.VertexCount);
        Assert.IsFalse(graph.ContainsVertex(2));
        Assert.IsFalse(graph.ContainsEdge(1, 2));
        Assert.IsFalse(graph.ContainsEdge(2, 3));
        Assert.AreEqual(0, graph.EdgeCount);
    }

    [TestMethod]
    public void TryGetEdgeWeight_MissingEdge_ShouldReturnFalseAndWeightZero()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();

        // Action
        var exists = graph.TryGetEdgeWeight(1, 2, out var weight);

        // Assert
        Assert.IsFalse(exists);
        Assert.AreEqual(0.0, weight);
    }

    [TestMethod]
    public void GetAdjacentVertices_MissingVertex_ShouldReturnEmptyCollection()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();

        // Action
        var adjacent = graph.GetAdjacentVertices(1);

        // Assert
        Assert.IsNotNull(adjacent);
        Assert.AreEqual(0, adjacent.Count);
    }

    [TestMethod]
    public void TryAddEdge_SelfLoop_ShouldThrowArgumentException()
    {
        // Arrange
        var graph = new GraphWithListOfWeight<int>();

        // Action
        try
        {
            graph.TryAddEdge(1, 1, 1.0);
            Assert.Fail("Ожидалось ArgumentException при добавлении петли.");
        }
        catch (ArgumentException ex)
        {
            // Assert
            Assert.IsInstanceOfType(ex, typeof(ArgumentException));
            StringAssert.Contains(ex.Message, "Петли не поддерживаются");
        }
    }
}
