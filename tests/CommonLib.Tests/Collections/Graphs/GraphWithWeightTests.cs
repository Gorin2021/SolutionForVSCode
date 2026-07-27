using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GraphWithWeightTests
{
    [TestMethod]
    public void TryAddVertex_NewVertex_ShouldIncreaseVertexCount()
    {
        var graph = new GraphWithWeight<int>();

        bool added = graph.TryAddVertex(1);

        Assert.IsTrue(added);
        Assert.AreEqual(1, graph.VertexCount);
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.Vertices.ToArray());
    }

    [TestMethod]
    public void TryAddEdge_UndirectedGraph_ShouldCreateBidirectionalEdgesAndIncrementEdgeCount()
    {
        var graph = new GraphWithWeight<int>(directed: false);

        bool added = graph.TryAddEdge(1, 2, 3.5);

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
        var graph = new GraphWithWeight<int>(directed: true);

        bool added = graph.TryAddEdge(1, 2, 2.0);

        Assert.IsTrue(added);
        Assert.AreEqual(2, graph.VertexCount);
        Assert.AreEqual(1, graph.EdgeCount);
        Assert.IsTrue(graph.ContainsEdge(1, 2));
        Assert.IsFalse(graph.ContainsEdge(2, 1));
    }

    [TestMethod]
    public void TryAddEdge_DuplicateEdge_ShouldReturnFalseAndUpdateWeight()
    {
        var graph = new GraphWithWeight<int>();

        bool firstAdd = graph.TryAddEdge(1, 2, 1.0);
        bool secondAdd = graph.TryAddEdge(1, 2, 4.2);

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
        var graph = new GraphWithWeight<int>();
        graph.TryAddEdge(1, 2, 1.0);

        bool removed = graph.TryRemoveEdge(1, 2);

        Assert.IsTrue(removed);
        Assert.AreEqual(0, graph.EdgeCount);
        Assert.IsFalse(graph.ContainsEdge(1, 2));
        Assert.IsFalse(graph.ContainsEdge(2, 1));
    }

    [TestMethod]
    public void TryRemoveVertex_ShouldRemoveVertexAndRelatedEdges()
    {
        var graph = new GraphWithWeight<int>();
        graph.TryAddEdge(1, 2, 1.0);
        graph.TryAddEdge(2, 3, 2.0);

        bool removed = graph.TryRemoveVertex(2);

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
        var graph = new GraphWithWeight<int>();

        bool exists = graph.TryGetEdgeWeight(1, 2, out var weight);

        Assert.IsFalse(exists);
        Assert.AreEqual(0.0, weight);
    }

    [TestMethod]
    public void GetAdjacentVertices_MissingVertex_ShouldReturnEmptyCollection()
    {
        var graph = new GraphWithWeight<int>();

        var adjacent = graph.GetAdjacentVertices(1);

        Assert.IsNotNull(adjacent);
        Assert.AreEqual(0, adjacent.Count);
    }

    [TestMethod]
    public void TryAddEdge_SelfLoop_ShouldThrowArgumentException()
    {
        var graph = new GraphWithWeight<int>();

        try
        {
            graph.TryAddEdge(1, 1, 1.0);
            Assert.Fail("Ожидалось ArgumentException при добавлении петли.");
        }
        catch (ArgumentException ex)
        {
            Assert.IsInstanceOfType(ex, typeof(ArgumentException));
        }
    }
}
