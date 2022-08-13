using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WpfGame.Extensions;
using WpfGame.Models;
using WpfGame.Models.Entities;
using WpfGame.Models.Map;

namespace WpfGame.AI
{
    public static class EntityExtensionsAi
    {
        public static void UpdateDirection(this Entity entity, Map map, ReactivePoint to)
        {
            bool IsWallPosition(Point previousPosition, Point currentPosition)
            {
                var direction = new Point(currentPosition.X - previousPosition.X, currentPosition.Y - previousPosition.Y);
                return entity.IsWallPosition(map, direction, previousPosition);
            }

            var start = new Point((int)entity.Position.X, (int)entity.Position.Y);
            var end = new Point((int)to.X, (int)to.Y);
            var path = Bfs.FindPath(start, end, IsWallPosition);
            if (path == null) return;
            var first = path.Skip(1).First();
            entity.Direction = new System.Windows.Point(first.X - start.X, first.Y - start.Y);
        }
    }
}