using System.Collections;

public class Node
{
    public int Value { get; set; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public Node(int value)
    {
        Value = value;
        Left = null;
        Right = null;
    }
}

public class BinaryTree : IEnumerable<int>
{
    private Node? root;

    public void Insert(int value)
    {
        if (root == null)
        {
            root = new Node(value);
        }
        else
        {
            InsertRecursively(root, value);
        }
    }

    private void InsertRecursively(Node node, int value)
    {
        if (value < node.Value)
        {
            if (node.Left == null)
            {
                node.Left = new Node(value);
            }
            else
            {
                InsertRecursively(node.Left, value);
            }
        }
        else
        {
            if (node.Right == null)
            {
                node.Right = new Node(value);
            }
            else
            {
                InsertRecursively(node.Right, value);
            }
        }
    }

    public IEnumerator<int> GetEnumerator()
    {
        return InOrder(root).GetEnumerator();
    }

    private IEnumerable<int> InOrder(Node? node)
    {
        if (node == null)
            yield break;

        foreach (var v in InOrder(node.Left))
            yield return v;

        yield return node.Value;

        foreach (var v in InOrder(node.Right))
            yield return v;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
