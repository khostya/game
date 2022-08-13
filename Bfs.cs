using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WpfGame.Extensions;
using WpfGame.Models;

namespace WpfGame;

public static class Bfs
{
    public delegate bool IsWallPositionDelegate(Point previousPosition, Point currentPosition);
    
    public static SinglyLinkedList<Point>? FindPath(Point start, Point end, IsWallPositionDelegate isWallPosition)
    {
        var queue = new PriorityQueue<SinglyLinkedList<Point>, double>();
        queue.Enqueue(new SinglyLinkedList<Point>(start), start.DistanceTo(end));
        var visited = new HashSet<Point> {start};
        while (queue.Count != 0)
        {
            var path = queue.Dequeue();
            var roundPoints = path.Value.GetRoundPoints();
            foreach (var roundPoint in roundPoints.Where(x => !visited.Contains(x) && !isWallPosition(path.Value, x)))
            {
                if (roundPoint.Equals(end))
                    return path;
                var newPath = new SinglyLinkedList<Point>(roundPoint, path);
                visited.Add(roundPoint);
                queue.Enqueue(newPath, roundPoint.DistanceTo(end));
            }
        }

        return null;
    }
}