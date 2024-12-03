using Godot;
using Thunder.Constants;

namespace Thunder;

public partial class Projectile : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        VisibleOnScreenNotifier.ScreenExited += QueueFree;
    }

    [Export] public float Speed { get; set; } = 500;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Position = Position with { X = Position.X + Speed * (float)delta };
    }

    public static Projectile Create(Vector2 pos)
    {
        var projectile = ResourceLoader.Load<PackedScene>(PrefabPaths.Item.Projectile).Instantiate<Projectile>();
        projectile.Position = pos;
        return projectile;
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public VisibleOnScreenNotifier2D VisibleOnScreenNotifier { get; set; } = null!;

    #endregion
}