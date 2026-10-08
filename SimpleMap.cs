namespace Collections;

public class SimpleMap<TKey, TValue>
{
    private class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Next;

        public Node(TKey key, TValue value, Node next)
        {
            Key = key;
            Value = value;
            Next = next;
        }
    }

    private Node[] buckets;
    private int count;

    public SimpleMap(int capacity = 16)
    {
        if (capacity < 1) capacity = 1;
        buckets = new Node[capacity];
    }

    public int Count => count;

    private static int HashOf(TKey key)
    {
        int hash = key == null ? 0 : key.GetHashCode();
        return hash & 0x7FFFFFFF;
    }

    public void Put(TKey key, TValue value)
    {
        int idx = HashOf(key) % buckets.Length;

        var node = buckets[idx];
        while (node != null)
        {
            if (Equals(node.Key, key))
            {
                node.Value = value;
                return;
            }
            node = node.Next;
        }

        buckets[idx] = new Node(key, value, buckets[idx]);
        count++;

        if (count > buckets.Length * 3 / 4)
            Resize(buckets.Length * 2);
    }

    public bool TryGet(TKey key, out TValue value)
    {
        int idx = HashOf(key) % buckets.Length;

        var node = buckets[idx];
        while (node != null)
        {
            if (Equals(node.Key, key))
            {
                value = node.Value;
                return true;
            }
            node = node.Next;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        int idx = HashOf(key) % buckets.Length;

        var node = buckets[idx];
        Node prev = null;
        while (node != null)
        {
            if (Equals(node.Key, key))
            {
                if (prev == null) buckets[idx] = node.Next;
                else prev.Next = node.Next;
                count--;
                return true;
            }
            prev = node;
            node = node.Next;
        }

        return false;
    }

    private void Resize(int newCapacity)
    {
        var old = buckets;
        buckets = new Node[newCapacity];

        for (int i = 0; i < old.Length; i++)
        {
            var node = old[i];
            while (node != null)
            {
                var next = node.Next;
                int idx = HashOf(node.Key) % newCapacity;
                node.Next = buckets[idx];
                buckets[idx] = node;
                node = next;
            }
        }
    }
}
