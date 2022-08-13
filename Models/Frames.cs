using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using WpfGame.Models.Entities;

namespace WpfGame.Models;

public class Frames
{
    private Dictionary<EntityState, CircularLinkedList<BitmapSource>> Source { get; }
    private EntityState? CurrentEntityState { get; set; }
    
    public Frames(Dictionary<EntityState, CircularLinkedList<BitmapSource>> source, TimeSpan updateInterval)
    {
        Source = source;
        Observable.Interval(updateInterval*5)
            .Where(_ => CurrentEntityState.HasValue)
            .Subscribe(_ => MoveNext());
    }

    private void MoveNext()
    {
        Source[CurrentEntityState!.Value] = Source[CurrentEntityState.Value].Next;
    }
    
    public BitmapSource GetCurrent(EntityState state)
    {
        CurrentEntityState = state;
        return Source[state].Value;
    }
}