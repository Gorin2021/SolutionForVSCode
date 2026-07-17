[TestClass]
public class BinarySearchTreeTests
{
    /// <summary>
    /// Проверяет, что новая структура данных имеет нулевое количество элементов.
    /// </summary>
    [TestMethod]
    public void NewTree_ShouldHaveZeroCount()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Assert
        Assert.AreEqual(0, tree.Count);
    }

    /// <summary>
    /// Проверяет, что добавление первого элемента успешно и увеличивает счётчик.
    /// </summary>
    [TestMethod]
    public void Add_FirstElement_ShouldReturnTrue()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        //Action
        bool added = tree.Add(10);

        //Assert
        Assert.IsTrue(added);
        Assert.AreEqual(1, tree.Count);
        Assert.IsTrue(tree.Contains(10));
    }

    /// <summary>
    /// Проверяет, что добавление дубликата возвращает false.
    /// </summary>
    [TestMethod]
    public void Add_Duplicate_ShouldReturnFalse()
    {
        //Arrange
        var tree = new BinarySearchTree<int>();

        //Action
        tree.Add(10);
        bool added = tree.Add(10);

        //Assert
        Assert.AreEqual(2, tree.Count);
        Assert.IsFalse(added);
    }

    /// <summary>
    /// Проверяет, что дубликат увеличивает логическое количество элементов дерева.
    /// </summary>
    [TestMethod]
    public void Add_Duplicate_ShouldIncreaseLogicalCount()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Action
        tree.Add(10);
        tree.Add(10);

        // Assert
        Assert.AreEqual(2, tree.Count);
        Assert.IsTrue(tree.Contains(10));
    }

    /// <summary>
    /// Проверяет, что дерево содержит все добавленные элементы после нескольких вставок.
    /// </summary>
    [TestMethod]
    public void Add_MultipleValues_ShouldContainAllValues()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Action
        tree.Add(50);
        tree.Add(20);
        tree.Add(70);
        tree.Add(10);
        tree.Add(30);

        // Assert
        Assert.IsTrue(tree.Contains(50));
        Assert.IsTrue(tree.Contains(20));
        Assert.IsTrue(tree.Contains(70));
        Assert.IsTrue(tree.Contains(10));
        Assert.IsTrue(tree.Contains(30));

        Assert.AreEqual(5, tree.Count);
    }

    /// <summary>
    /// Проверяет, что поиск отсутствующего значения возвращает false.
    /// </summary>
    [TestMethod]
    public void Contains_MissingValue_ShouldReturnFalse()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Action
        tree.Add(10);
        tree.Add(20);
        tree.Add(30);

        // Assert
        Assert.IsFalse(tree.Contains(99));
    }

    /// <summary>
    /// Проверяет, что прямой обход дерева возвращает элементы в отсортированном порядке.
    /// </summary>
    [TestMethod]
    public void InOrderTraversal_ShouldReturnSortedSequence()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Action
        tree.Add(50);
        tree.Add(20);
        tree.Add(70);
        tree.Add(10);
        tree.Add(30);
        tree.Add(60);
        tree.Add(80);

        int[] result = tree.InOrderTraversal().ToArray();

        // Assert
        CollectionAssert.AreEqual(
            new[]
            {
            10,
            20,
            30,
            50,
            60,
            70,
            80
            },
            result);
    }

    /// <summary>
    /// Проверяет, что обход дерева сохраняет дублирующиеся значения.
    /// </summary>
    [TestMethod]
    public void InOrderTraversal_ShouldReturnDuplicates()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Action
        tree.Add(10);
        tree.Add(10);
        tree.Add(10);
        tree.Add(20);

        int[] result = tree.InOrderTraversal().ToArray();

        // Assert
        CollectionAssert.AreEqual(
            new[]
            {
            10,
            10,
            10,
            20
            },
            result);
    }

    /// <summary>
    /// Проверяет, что обход пустого дерева возвращает пустую последовательность.
    /// </summary>
    [TestMethod]
    public void InOrderTraversal_EmptyTree_ShouldReturnEmptySequence()
    {
        // Arrange
        var tree = new BinarySearchTree<int>();

        // Assert
        Assert.AreEqual(0, tree.InOrderTraversal().Count());
    }

    /// <summary>
    /// Проверяет, что дерево корректно работает с элементами типа string.
    /// </summary>
    [TestMethod]
    public void Tree_ShouldWorkWithStrings()
    {
        //Arrange
        var tree = new BinarySearchTree<string>();

        //Action
        tree.Add("Charlie");
        tree.Add("Alice");
        tree.Add("Bob");

        string[] result = tree.InOrderTraversal().ToArray();

        //Assert
        CollectionAssert.AreEqual(
            new[]
            {
            "Alice",
            "Bob",
            "Charlie"
            },
            result);
    }


    private sealed class ReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

    /// <summary>
    /// Проверяет, что дерево использует переданный пользовательский компаратор.
    /// </summary>
    [TestMethod]
    public void Tree_ShouldUseCustomComparer()
    {
        // Arrange
        var tree = new BinarySearchTree<int>(new ReverseComparer());

        // Action
        tree.Add(10);
        tree.Add(20);
        tree.Add(30);

        int[] result = tree.InOrderTraversal().ToArray();
        
        // Assert
        CollectionAssert.AreEqual(
            new[]
            {
            30,
            20,
            10
            },
            result);
    }
}