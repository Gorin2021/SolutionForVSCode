using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathOnGraphTests
{
    [TestMethod]
    // Проверяет, что начальная вершина добавляется и её вес равен нулю.
    public void AddStartVertex_ShouldAddFirstVertexWithZeroWeight()
    {
        // Arrange
        var path = new PathOnGraph<int>();

        // Action
        path.AddStartPathEdge(1);

        // Assert
        Assert.AreEqual(1, path.Count);
        Assert.AreEqual(1, path.Last.Target);
        Assert.AreEqual(0.0, path.Last.PathWeight);
    }

    [TestMethod]
    // Проверяет накопление веса при добавлении следующих ребер пути.
    public void Add_ShouldAppendNextEdgeWithCumulativeWeight()
    {
        // Arrange
        var path = new PathOnGraph<int>();
        path.AddStartPathEdge(1);

        // Action
        path.Add(new Edge<int>(2, 5.5));
        path.Add(new Edge<int>(3, 2.5));

        // Assert
        Assert.AreEqual(3, path.Count);
        Assert.AreEqual(3, path.Last.Target);
        Assert.AreEqual(8.0, path.Last.PathWeight);
    }

    [TestMethod]
    // Проверяет, что клон пути не изменяет оригинал и содержит собственные вершины.
    public void Clone_ShouldCreateIndependentCopy()
    {
        // Arrange
        var path = new PathOnGraph<int>();
        path.AddStartPathEdge(1);
        path.Add(new Edge<int>(2, 3.0));

        // Action
        var clone = path.Clone();
        clone.Add(new Edge<int>(3, 4.0));

        // Assert
        Assert.AreEqual(2, path.Count);
        Assert.AreEqual(3, clone.Count);
        Assert.AreEqual(3, clone.Last.Target);
        Assert.AreEqual(7.0, clone.Last.PathWeight);
    }

    [TestMethod]
    // Проверяет успешный поиск ребра пути по целевой вершине.
    public void TryGetPathEdge_ShouldReturnTrueWhenVertexExists()
    {
        // Arrange
        var path = new PathOnGraph<int>();
        path.AddStartPathEdge(1);
        path.Add(new Edge<int>(2, 2.0));
        path.Add(new Edge<int>(3, 4.0));

        // Action
        var found = path.TryGetPathEdge(2, out var pathEdge);

        // Assert
        Assert.IsTrue(found);
        Assert.AreEqual(2, pathEdge.Target);
        Assert.AreEqual(2.0, pathEdge.PathWeight);
    }

    [TestMethod]
    // Проверяет, что поисковый путь работает с ссылочным типом вершины.
    public void TryGetPathEdge_ShouldReturnTrueWhenStringVertexExists()
    {
        // Arrange
        var path = new PathOnGraph<string>();
        path.AddStartPathEdge("A");
        path.Add(new Edge<string>("B", 1.5));
        path.Add(new Edge<string>("C", 2.5));

        // Action
        var found = path.TryGetPathEdge("B", out var pathEdge);

        // Assert
        Assert.IsTrue(found);
        Assert.AreEqual("B", pathEdge.Target);
        Assert.AreEqual(1.5, pathEdge.PathWeight);
    }

    [TestMethod]
    // Проверяет, что ссылочный тип пути корректно возвращает false для отсутствующей вершины.
    public void TryGetPathEdge_ShouldReturnFalseWhenStringVertexMissing()
    {
        // Arrange
        var path = new PathOnGraph<string>();
        path.AddStartPathEdge("A");
        path.Add(new Edge<string>("B", 1.5));

        // Action
        var found = path.TryGetPathEdge("C", out var pathEdge);

        // Assert
        Assert.IsFalse(found);
        Assert.AreEqual(default, pathEdge);
    }
}
