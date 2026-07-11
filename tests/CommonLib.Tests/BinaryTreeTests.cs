using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BinaryTreeTests
{
    [TestClass]
    public class BinaryTreeTests
    {
        [TestMethod]
        public void GetEnumerator_ShouldTraverseTreeLeftToRight_InOrder()
        {
            // Arrange
            var tree = new BinaryTree();
            tree.Insert(5);
            tree.Insert(3);
            tree.Insert(7);
            tree.Insert(2);
            tree.Insert(4);
            tree.Insert(6);
            tree.Insert(8);

            var items = new List<int>();

            // Act
            foreach (var value in tree)
            {
                items.Add(value);
            }

            // Assert
            CollectionAssert.AreEqual(new[] { 2, 3, 4, 5, 6, 7, 8 }, items);
        }
    }
}
