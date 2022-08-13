using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Splat;
using Size = System.Windows.Size;
using WpfGame.Models.Entities;

namespace WpfGame.Models.Map;

public class Map
{
    private readonly MapCell[][] _map;
    public int Height => _map.Length;
    public int Width => _map[0].Length;
    public Point PlayerPosition { get; }
    public (MapCell Cell, int X, int Y)[] EntitiesPositions;
    
    public Map(MapCell[][] map)
    {
        _map = map;
        var player =  _map.SelectMany((row, y) => row.Select((cell, x) => (cell, x, y)))
            .First(x => x.cell == MapCell.Player);
        PlayerPosition = new Point(player.x, player.y);
        EntitiesPositions = _map.SelectMany((row, y) => row.Select((cell, x) => (cell, x, y)))
            .Where(x => x.cell == MapCell.Crab).ToArray();
    }


    public MapCell this[int column, int row] => _map[column][row];
    
    public IEnumerable<(int X, int Y, MapCell Cell)> Cells => _map
        .SelectMany((row, y) => row.Select((cell, x) => (x, y, cell)));

    public static Point GetPosition(Point position, Point direction)
        {
            var cellSizeInView = Locator.Current.GetService<Size>();
            var centerX = position.X + cellSizeInView.Width / 2;
            var centerY = position.Y + cellSizeInView.Height / 2;
            Point currentPosition;
            if (direction.Y == -1 || direction.X == -1)
                currentPosition = new Point(
                    (int) Math.Ceiling(centerX / cellSizeInView.Width),
                    (int) Math.Ceiling(centerY / cellSizeInView.Height));
            else 
                currentPosition = new Point(
                    (int) Math.Floor(centerX / cellSizeInView.Width), 
                    (int) Math.Floor(centerY / cellSizeInView.Height));
            return currentPosition;
        }
        
    public bool InBound(System.Windows.Point point)
    {
        return point.X < Width && point.X >= 0 &&
               point.Y < Height && point.Y >= 0;
    }
}