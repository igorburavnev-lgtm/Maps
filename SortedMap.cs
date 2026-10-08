namespace Collections;

public class SortedMap<TKey, TValue> where TKey : IComparable<TKey>
{
    private class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Left;
        public Node Right;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    private Node root;
    private int count;

    public int Count => count;

    public void Put(TKey key, TValue value)
    {
        if (root == null)
        {
            root = new Node(key, value);
            count = 1;
            return;
        }

        var cur = root;
        while (true)
        {
            int cmp = key.CompareTo(cur.Key);
            if (cmp == 0)
            {
                cur.Value = value;
                return;
            }
            if (cmp < 0)
            {
                if (cur.Left == null)
                {
                    cur.Left = new Node(key, value);
                    count++;
                    return;
                }
                cur = cur.Left;
            }
            else
            {
                if (cur.Right == null)
                {
                    cur.Right = new Node(key, value);
                    count++;
                    return;
                }
                cur = cur.Right;
            }
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        var cur = root;
        while (cur != null)
        {
            int cmp = key.CompareTo(cur.Key);
            if (cmp == 0)
            {
                value = cur.Value;
                return true;
            }
            cur = cmp < 0 ? cur.Left : cur.Right;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        if (!TryGet(key, out _)) return false;
        root = RemoveNode(root, key);
        count--;
        return true;
    }

    private static Node RemoveNode(Node node, TKey key)
    {
        if (node == null) return null;

        int cmp = key.CompareTo(node.Key);
        if (cmp < 0)
        {
            node.Left = RemoveNode(node.Left, key);
        }
        else if (cmp > 0)
        {
            node.Right = RemoveNode(node.Right, key);
        }
        else
        {
            if (node.Left == null) return node.Right;

            if (node.Right == null) return node.Left;

            var successor = FindMin(node.Right);
            node.Key = successor.Key;
            node.Value = successor.Value;
            node.Right = RemoveNode(node.Right, successor.Key);
        }

        return node;
    }

    private static Node FindMin(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    public IEnumerable<TKey> KeysInOrder()
    {
        return InOrder(root);
    }

    private static IEnumerable<TKey> InOrder(Node node)
    {
        if (node == null) yield break;
        foreach (var k in InOrder(node.Left)) yield return k;
        yield return node.Key;
        foreach (var k in InOrder(node.Right)) yield return k;
    }

    public TKey MinKey()
    {
        if (root == null) throw new InvalidOperationException("Map is empty");
        var node = root;
        while (node.Left != null) node = node.Left;
        return node.Key;
    }

    public TKey MaxKey()
    {
        if (root == null) throw new InvalidOperationException("Map is empty");
        var node = root;
        while (node.Right != null) node = node.Right;
        return node.Key;
    }
}
