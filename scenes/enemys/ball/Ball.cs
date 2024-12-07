using Godot;
using Thunder.Classes;
using Thunder.Globals.Extensions;

namespace Thunder.Enemys;

public partial class Ball : Node2D
{
    [Export] public float Speed { get; set; } = -100;

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public HurtBox HurtBox { get; set; } = null!;

    #endregion

    #region Health

    private float _health = 3;

    [Export]
    public float Health
    {
        get => _health;
        set => SetHealth(value);
    }

    private async void SetHealth(float value)
    {
        await this.EnsureReadyAsync();
        _health = value;
        if (_health <= 0)
            Die();
    }

    private void Die()
    {
        QueueFree();
    }

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        base._Ready();
        HurtBox.Hurt += OnHurt;
    }

    private void OnHurt(HitBox hitBox)
    {
        if (hitBox.Owner is not Projectile projectile) return;
        Health -= projectile.Damage;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Position = Position with { X = Position.X + Speed * (float)delta };
    }

    #endregion
}