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

    [Export] public AnimationPlayer AnimationPlayer { get; set; } = null!;

    #endregion

    #region 生命值和存活

    #region IsAlive

    private bool _isAlive;

    [Export]
    public bool IsAlive
    {
        get => _isAlive;
        set => SetIsAlive(value);
    }

    private async void SetIsAlive(bool value)
    {
        await this.EnsureReadyAsync();
        _isAlive = value;
        HurtBox.SetDeferred(Area2D.PropertyName.Monitorable, IsAlive);
    }

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
        IsAlive = false;
        AnimationPlayer.Play("die");
        CleanUp();
    }

    private async void CleanUp()
    {
        var animationSignal = ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);

        // TODO: 添加音效，并等待音效播放完成后再销毁
        // var sfxSignal = ...
        await SignalExtensions.WhenAll(animationSignal);
        Destroy();
    }

    private void Destroy()
    {
        QueueFree();
    }

    #endregion

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        base._Ready();
        IsAlive = true;
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