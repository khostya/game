namespace WpfGame.Models.Entities;

public class Player : Entity
{
    public Player(ReactivePoint position) : base(position, EntityType.Player, 20)
    {
    }

    public void Shoot() => EntityState = EntityState.Shoot;
    
    public void ChangeState(EntityState entityState)
    {
        if (EntityState is EntityState.Attack or EntityState.Shoot) return;
        EntityState = entityState;
    }
}