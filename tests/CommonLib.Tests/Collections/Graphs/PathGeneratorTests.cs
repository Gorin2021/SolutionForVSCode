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
}