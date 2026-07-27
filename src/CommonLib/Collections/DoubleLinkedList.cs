using System.Collections;
using System.Collections.Generic;

// Copied from src/ConsoleApp1/DoubleLinkedList.cs
public sealed class DoubleLinkedList<T> : IEnumerable<T>
{
    private Node? _head;
    private Node? _tail;
    private int _version;
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    public int Count { get; private set; }

    private sealed class Node
    {
        public T Value { get; private set; }
        public Node? Next { get; private set; }
        public Node? Previous { get; private set; }

        public Node(T value) => Value = value;

        public void SetNext(Node? next) => Next = next;

        public void SetPrevious(Node? previous) => Previous = previous;

        public void Detach()
        {
            Next = null;
            Previous = null;
            Value = default!;
        }
    }

    public void AddFirst(T value) => InsertFirst(new Node(value));

    public void AddLast(T value) => InsertLast(new Node(value));

    public bool Remove(T value)
    {
        var node = FindNode(value);
        if (node == null)
            return false;

        RemoveNode(node);
        return true;
    }

    public bool Contains(T value) => FindNode(value) != null;

    public void Clear()
    {
        var current = _head;
        while (current != null)
        {
            var next = current.Next;
            current.Detach();
            current = next;
        }

        _head = null;
        _tail = null;
        Count = 0;
        _version++;
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
        private readonly DoubleLinkedList<T> _list;
        private Node? _nextNode;
        private T? _currentValue;
        private readonly int _version;

        internal Enumerator(DoubleLinkedList<T> list)
        {
            _list = list;
            _nextNode = list._head;
            _currentValue = default;
            _version = list._version;
        }

        public T Current => _currentValue!;

        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified after the enumerator was created.");

            if (_nextNode == null)
            {
                _currentValue = default!;
                return false;
            }

            _currentValue = _nextNode.Value;
            _nextNode = _nextNode.Next;
            return true;
        }

        public void Reset() => throw new NotSupportedException("Reset() is not supported.");

        public void Dispose()
        {
        }
    }

    private void InsertFirst(Node node)
    {
        if (_head == null)
            InsertIntoEmptyList(node);
        else
            InsertIntoHead(node);
    }

    private void InsertLast(Node node)
    {
        if (_tail == null)
            InsertIntoEmptyList(node);
        else
            InsertIntoTail(node);
    }

    private void InsertIntoEmptyList(Node node)
    {
        _head = _tail = node;
        OnInserted();
    }

    private void InsertIntoHead(Node node)
    {
        node.SetNext(_head);
        _head!.SetPrevious(node);
        _head = node;
        OnInserted();
    }

    private void InsertIntoTail(Node node)
    {
        node.SetPrevious(_tail);
        _tail!.SetNext(node);
        _tail = node;
        OnInserted();
    }

    private void OnInserted()
    {
        Count++;
        _version++;
    }

    private Node? FindNode(T value)
    {
        for (var node = _head; node != null; node = node.Next)
        {
            if (Comparer.Equals(node.Value, value))
                return node;
        }

        return null;
    }

    private void RemoveNode(Node nodeToRemove)
    {
        if (nodeToRemove.Previous != null)
            nodeToRemove.Previous.SetNext(nodeToRemove.Next);
        else
            _head = nodeToRemove.Next;

        if (nodeToRemove.Next != null)
            nodeToRemove.Next.SetPrevious(nodeToRemove.Previous);
        else
            _tail = nodeToRemove.Previous;

        nodeToRemove.Detach();
        OnRemoved();
    }

    private void OnRemoved()
    {
        Count--;
        _version++;
    }
}
