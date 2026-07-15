public sealed class BinarySearchTree<T>
{
    private sealed class Node
    {
        public T Value;
        public Node? Left;
        public Node? Right;
        public Node? Parent;

        public int Height;
        public int DuplicateCount;

        public Node(T value, Node? parent)
        {
            Value = value;
            Parent = parent;

            Left = null;
            Right = null;

            Height = 1;
            DuplicateCount = 1;
        }
    }

    private Node? _root;
    private int _count;
    private readonly IComparer<T> _comparer;

    public BinarySearchTree(IComparer<T>? comparer = null)
    {
        _root = null;
        _count = 0;
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public int Count => _count;

    public bool Add(T value)
    {
        if (_root is null)
        {
            _root = new Node(value, null);
            _count++;
            return true;
        }

        Node? current = _root;
        Node? parent = null;

        while (true)
        {
            parent = current;

            int comparison = _comparer.Compare(value, current.Value);

            if (comparison < 0)
            {
                current = current.Left;
            }
            else if (comparison > 0)
            {
                current = current.Right;
            }
            else
            {
                current.DuplicateCount++;
                _count++;
                return false;
            }

            //Дошли до конца дерева, вставляем новый узел
            Node newNode = new Node(value, parent);

            if (_comparer.Compare(value, parent.Value) < 0)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }
            return true;
        }
    }
}