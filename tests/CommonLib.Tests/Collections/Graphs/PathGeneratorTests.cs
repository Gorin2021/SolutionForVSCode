[TestClass]
public class PathGeneratorTests
{
    [DynamicData(nameof(LoadEdges))]
    [TestMethod]
    public void GeneratePathShouldPathWithValidWeight(GraphWithListOfWeight<int> graph)
    {
        //Arrange
        var pathGenerator = new PathGenerator<int>(graph: graph);

        //Action
        pathGenerator.GeneratePaths(startVertex: 1, endVertex: 10);

        //Assert
        Assert.AreEqual(23, pathGenerator.GeneratedPath?.Last.PathWeight);
    }

    [DynamicData(nameof(LoadEdges))]
    [TestMethod]
    public void GeneratePathShouldEndVertexesValid(GraphWithListOfWeight<int> graph)
    {
        //Arrange
        var pathGenerator = new PathGenerator<int>(graph: graph);

        //Action
        pathGenerator.GeneratePaths(startVertex: 1, endVertex: 10);

        //Assert
        Assert.AreEqual(10, pathGenerator.GeneratedPath?.Last.Target);
    }


    private static GraphWithListOfWeight<int>[] LoadEdges()
    {
        var graphWithListOfWeight = new GraphWithListOfWeight<int>();
        graphWithListOfWeight.TryAddEdge(src: 1, dest: 2, weight: 7); // 1 -> 2 (weight: 0 + 7 = 7)
        graphWithListOfWeight.TryAddEdge(src: 1, dest: 5, weight: 1);
        graphWithListOfWeight.TryAddEdge(src: 1, dest: 3, weight: 4);
        graphWithListOfWeight.TryAddEdge(src: 2, dest: 5, weight: 4); // 2 -> 5 (weight: 7+4 = 11)
        graphWithListOfWeight.TryAddEdge(src: 2, dest: 4, weight: 1);
        graphWithListOfWeight.TryAddEdge(src: 5, dest: 10, weight: 400); // ????? 5 -> 10 (weight: 411) 
        graphWithListOfWeight.TryAddEdge(src: 5, dest: 3, weight: 2);   // 5 -> 3 (weight: 2 + 11 = 13)
        graphWithListOfWeight.TryAddEdge(src: 3, dest: 4, weight: 4); // 3 -> 4 (weight: 13 + 4 = 17)
        graphWithListOfWeight.TryAddEdge(src: 4, dest: 9, weight: 1); // 4 -> 9 (weight: 17 + 1 = 18)
        graphWithListOfWeight.TryAddEdge(src: 9, dest: 10, weight: 5); // !!!!! 9 -> 10 (weight: 18 + 5 = 23)
        graphWithListOfWeight.TryAddEdge(src: 9, dest: 6, weight: 2); // 9 -> 6 (weight: 18 + 2 = 20)
        graphWithListOfWeight.TryAddEdge(src: 6, dest: 8, weight: 2); // 6 -> 8 (weight: 20 + 2 = 22)
        graphWithListOfWeight.TryAddEdge(src: 8, dest: 7, weight: 3); // 8 -> 7 (weight: 22 + 3 = 25)
        return [graphWithListOfWeight];

    }

    [TestMethod]
    public void GeneratePath_ShouldChooseShortestRouteInLargeGraph()
    {
        // Arrange
        var graph = CreateLargeGraphWithSpecialRoutes();
        var pathGenerator = new PathGenerator<int>(graph);

        // Action
        pathGenerator.GeneratePaths(startVertex: 256, endVertex: 56000);

        // Assert
        Assert.IsNotNull(pathGenerator.GeneratedPath);
        Assert.AreEqual(56000, pathGenerator.GeneratedPath?.Last.Target);
        Assert.AreEqual(10.0, pathGenerator.GeneratedPath?.Last.PathWeight);
        Assert.AreEqual(31, pathGenerator.GeneratedPath?.Count);
    }

    private static GraphWithListOfWeight<int> CreateLargeGraphWithSpecialRoutes()
    {
        var graph = new GraphWithListOfWeight<int>(directed: false);

        // Создаём 1000000 вершин заранее.
        for (int i = 1; i <= 1_000_000; i++)
        {
            graph.TryAddVertex(i);
        }

        // Специальный путь с наименьшим весом через вершину 99999.
        graph.TryAddEdge(256, 99999, 0.0);
        var current = 99999;
        for (int value = 30_001; value <= 30_001 + 5 * 27; value += 5)
        {
            graph.TryAddEdge(current, value, 0.0);
            current = value;
        }
        graph.TryAddEdge(current, 56_000, 10.0);

        // Специальный путь через вершину 10, 100 вершин, вес 1000.
        graph.TryAddEdge(256, 10, 20.0);
        current = 10;
        for (int value = 20_001; value <= 20_001 + 5 * 96; value += 5)
        {
            graph.TryAddEdge(current, value, 10.0);
            current = value;
        }
        graph.TryAddEdge(current, 56_000, 10.0);

        // Специальный путь через вершину 1, 1000 вершин, вес 10000.
        graph.TryAddEdge(256, 1, 20.0);
        current = 1;
        for (int value = 10_001; value <= 10_001 + 5 * 997; value += 5)
        {
            graph.TryAddEdge(current, value, 10.0);
            current = value;
        }
        graph.TryAddEdge(current, 56_000, 10.0);

        // Четвёртый путь через вершину 3, 4 вершины, вес 5000.
        graph.TryAddEdge(256, 3, 2000.0);
        graph.TryAddEdge(3, 10_000, 2000.0);
        graph.TryAddEdge(10_000, 56_000, 1000.0);

        // Общие ребра, чтобы каждая вершина имела от 0 до 100 смежных вершин.
        for (int i = 1; i <= 1_000_000; i++)
        {
            if (i + 1 <= 1_000_000)
                graph.TryAddEdge(i, i + 1, 1000.0);

            if (i + 2 <= 1_000_000)
                graph.TryAddEdge(i, i + 2, 1000.0);

            if (i + 10 <= 1_000_000)
                graph.TryAddEdge(i, i + 10, 1000.0);

            if (i % 50 == 0 && i + 25 <= 1_000_000)
                graph.TryAddEdge(i, i + 25, 2000.0);
        }

        return graph;
    }
}