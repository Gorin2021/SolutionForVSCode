[TestClass]
public class BinarySearchTreeTests
{
    [TestMethod]
    public void NewTree_ShouldHaveZeroCount()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Assert
        Assert.AreEqual(0, tree.Count);
    }
}