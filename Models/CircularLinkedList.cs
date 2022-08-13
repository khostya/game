using System.Collections.Generic;

namespace WpfGame.Models;

public class CircularLinkedList<T>
{
    public CircularLinkedList<T> Next { get; private set; } = null!;
    public T Value { get; } = default!;

    private CircularLinkedList(T value) => Value = value;

    public CircularLinkedList(IEnumerable<T> values)
    {
        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext()) return;
        Value = enumerator.Current;
        var current = this;
        var first = current;
        Value = current.Value;
        while (enumerator.MoveNext())
        {
            current.Next = new CircularLinkedList<T>(enumerator.Current);
            current = current.Next;
        }

        current.Next = first;
    }
}