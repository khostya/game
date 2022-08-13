using System;
using System.Collections.Generic;
using System.Linq;
using WpfGame.Models;

namespace WpfGame.Extensions;

public static class IEnumerableExtensions
{
    public static CircularLinkedList<T> ToCircularLinkedList<T>(this IEnumerable<T> source)
    {
        return new CircularLinkedList<T>(source);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }
    
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        foreach (var (item, i) in source.Select((x, i) => (x, i))) 
            action(item, i);
    }
}