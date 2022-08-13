using System.Collections;
using System.Collections.Generic;

namespace WpfGame.Models;

public class SinglyLinkedList<T> : IEnumerable<T>
{
    public T Value { get; }
    
    public SinglyLinkedList<T>? Previous { get; }

    public SinglyLinkedList(T value, SinglyLinkedList<T>? previous = null) 
    {
        Value = value;
        Previous = previous;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var linkedList = new LinkedList<T>();
        linkedList.AddFirst(Value);
        var previous = Previous;
        while (previous != null)
        {
            linkedList.AddFirst(previous.Value);
            previous = previous.Previous;
        }

        foreach (var value in linkedList) yield return value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}