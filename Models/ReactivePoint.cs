using System;
using System.Collections.Generic;
using ReactiveUI;

namespace WpfGame.Models;

public class ReactivePoint : ReactiveObject
{
    private double _x;
    private double _y;

    public double X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref _x, value);
    }
    
    public double Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public ReactivePoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static ReactivePoint Empty => new(0, 0);
    
    public bool IsEmpty => X == 0 && Y == 0;

    public void Offset(ReactivePoint reactivePoint) => Offset(reactivePoint.X, reactivePoint.Y);
    
    public void Offset(double x, double y)
    {
        X += x;
        Y += y;
    }
    
    public double DistanceTo(ReactivePoint to)
    {
        return Math.Abs(X - to.X) + Math.Abs(Y - to.Y);
    }
    
    public override string ToString() => $"X = {X}, Y = {Y}";
}