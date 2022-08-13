using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Splat;
using WpfGame.AI;
using WpfGame.Extensions;
using WpfGame.Models.Entities;
using WpfGame.Models.Map;
namespace WpfGame.Models.Game;

public class Game
{
    public Player Player { get; }
    public List<Entity> Entities { get; }
    public List<Bullet> Bullets { get; }
    public Map.Map Map { get; }
    private Size CellSizeInView { get; }

    public Game(MapCell[][] map)
    {
        CellSizeInView = Locator.Current.GetService<Size>();
        Map = new Map.Map(map);
        Entities = new List<Entity>();
        Bullets = new List<Bullet>();
        foreach (var (cell, x, y) in Map.EntitiesPositions)
        {
            Entity entity;
            var pos = new ReactivePoint(x * CellSizeInView.Width, y * CellSizeInView.Height);
            if (cell != MapCell.Crab) throw new NotSupportedException();
            entity = new Crab(pos);
            Entities.Add(entity);
        }
        
        Player = new Player(new ReactivePoint(Map.PlayerPosition.X * CellSizeInView.Width,
            Map.PlayerPosition.Y * CellSizeInView.Height));
    }


    public void AddBullet(Bullet bullet) => Bullets.Add(bullet);

    public void MoveBullets()
    {
        Bullets.ForEach(x => x.Move(Map));
    }
    public void MoveEntitiesAndFighting()
    {
        MovePlayer();
        MoveEntities();
        Fighting();
    }

    private void MoveEntities()
    {
        var tasks = Entities.Where(entity => entity.EntityState != EntityState.Death)
            .Select(entity =>
            {
                if (entity.DistanceTo(Player) > CellSizeInView.Width * 6) entity.Stand();
                if (entity.DistanceTo(Player) < CellSizeInView.Height)
                {
                    entity.Attack();
                    return Task.CompletedTask;
                }

                return Task.Run(() =>
                {
                    entity.UpdateDirection(Map, Player.Position);
                    entity.Move(Map);
                    entity.Run();
                });
            }).ToArray();
        Task.WaitAll(tasks);
    }
    
    private void MovePlayer()
    {
        if (!Player.Direction.Equals(new Point(0, 0)))
        {
            Player.Move(Map);
            Player.Run();
        }
        else
            Player.ChangeState(EntityState.Standing);
    }
    
    private void Fighting()
    {
        Entities.Where(x => x.EntityState == EntityState.Attack).ForEach(x => x.Attack(Player));
        Entities.ForEach(entity => Bullets.ForEach(bullet => bullet.Hit(entity)));
        if (Player.EntityState == EntityState.Attack)
            Entities.Where(x => x.DistanceTo(Player) < CellSizeInView.Width)
                .ForEach(x => Player.Attack(x));
    }
}