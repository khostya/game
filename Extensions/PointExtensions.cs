using System;
using System.Collections.Generic;
using System.Drawing;

namespace WpfGame.Extensions;

public static class PointExtensions
{
    public static IEnumerable<Point> GetRoundPoints(this Point point)
    {
        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
            if (!(dx == dy && dx == 0))
                yield return new Point(point.X + dx, point.Y + dy);
    }
    
    public static int DistanceTo(this Point from, Point to)
    {
        return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
    }
}