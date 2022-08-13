using System;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using Splat;
using WpfGame.Extensions;
using WpfGame.Models.Entities;
using WpfGame.Models.Map;

namespace WpfGame.Models;

public class Bullet
{
    public ReactivePoint Position { get; }
    public Point Direction { get; }
    private bool IsHitEntity { get; set; }
    public EntityState BulletState { get; private set; }
    private int Damage { get; }

    public Bullet(ReactivePoint position, Point direction)
    {
        Position = position;
        Damage = 5;
        IsHitEntity = false;
        Direction = direction;
        BulletState = EntityState.Run;
    }

    public void Hit(Entity entity)
    {
        if (!CanHit(entity) || IsHitEntity) return;
        BulletState = EntityState.Death;
        IsHitEntity = true;
        entity.Health -= Damage;
        if (entity.Health == 0) entity.Death();
    }
    
    private bool CanHit(Entity entity)
    {
        var cellSize = Locator.Current.GetService<Size>();
        var centerX = entity.Position.X + cellSize.Width / 2;
        var centerY = entity.Position.Y + cellSize.Height / 2;
        return new ReactivePoint(centerX, centerY + 15).DistanceTo(Position) < cellSize.Height / 2;
    }
    
    public void Move(Map.Map map)
    {
        if (!CanMove(map) || BulletState == EntityState.Death)
        {
            BulletState = EntityState.Death;
            return;
        }

        Position.Offset(Direction.X, Direction.Y);
    }

    private bool CanMove(Map.Map map)
    {
        var cellSize = Locator.Current.GetService<Size>();
        var centerX = Position.X + cellSize.Width / 2;
        var centerY = Position.Y + cellSize.Height / 2;
        Point currentPosition;
        if (Direction.Y == -1 || Direction.X == -1)
            currentPosition = new Point(
                (int) Math.Ceiling(centerX / cellSize.Width), (int) Math.Ceiling(centerY / cellSize.Height));
        else 
            currentPosition = new Point(
                (int) Math.Floor(centerX / cellSize.Width), (int) Math.Floor(centerY / cellSize.Height));
        var nextPosition = new Point(currentPosition.X + Direction.X, currentPosition.Y + Direction.Y);
        if (!map.InBound(nextPosition)) return false;
        return map[(int)nextPosition.Y, (int)nextPosition.X] != MapCell.Wall;
    }
}