using System;
using System.Windows;
using System.Windows.Forms;
using Splat;
using WpfGame.Extensions;
using WpfGame.Models.Map;

namespace WpfGame.Models.Entities;

public class Entity
{
    public ReactivePoint Position { get; }
    public Point Direction { get; set; }
    public EntityType EntityType { get; }
    private int _health;
    public int Health
    {
        get => _health;
        set => _health = value <= 0 ? 0 : value;
    }

    public EntityState EntityState { get; protected set; }
    private int Damage { get; }

    protected Entity(ReactivePoint position, EntityType entityType, int damage)
    {
        Damage = damage;
        Position = position;
        EntityType = entityType;
        Direction = new Point(0, 0);
        Health = 100;
        EntityState = EntityState.Standing;
    }

    public double DistanceTo(Entity entity) => Position.DistanceTo(entity.Position);

    public void Move(Map.Map map)
    {
        if (EntityState == EntityState.Attack || !CanMove(map) || EntityState == EntityState.Death) return;
        Position.Offset(Direction.X, Direction.Y);
    }

    public bool IsWallPosition(Map.Map map, System.Drawing.Point direction, System.Drawing.Point position)
    {
        var currentPosition = Map.Map.GetPosition(position, direction);
        var nextPosition = new Point(currentPosition.X + direction.X, currentPosition.Y + direction.Y);
        if (!map.InBound(nextPosition)) return false;
        return map[(int)nextPosition.Y, (int)nextPosition.X] == MapCell.Wall;
    }

    public bool CanMove(Map.Map map)
    {
        var direction = new System.Drawing.Point((int) Direction.X, (int) Direction.Y);
        var position = new System.Drawing.Point((int)Position.X, (int)Position.Y);
        return !IsWallPosition(map, direction, position);
    }

    public void Run() => EntityState = EntityState.Run;

    public void Attack() => EntityState = EntityState.Attack;
    
    public void Attack(Entity entity)
    {
        entity.Health -= Damage;
        if (entity.Health == 0) entity.Death();
    }
    
    public void Death() => EntityState = EntityState.Death;

    public void StopAttack() => EntityState = Direction.Equals(new Point(0, 0)) ? EntityState.Standing : EntityState.Run;

    public void Stand() => EntityState = EntityState.Standing;
}
