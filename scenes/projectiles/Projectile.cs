using Godot;
using Thunder.Classes;
using Thunder.Constants;

namespace Thunder;

public partial class Projectile : Area2D
{
    public static Projectile Create(Vector2 initPosition, float initRotation,
        PositionGeneratorDelegate positionGeneratorDelegate, RotationGeneratorDelegate rotationGeneratorDelegate)
    {
        var projectile = ResourceLoader.Load<PackedScene>(PrefabPaths.Item.Projectile).Instantiate<Projectile>();
        projectile.StartPosition = initPosition;
        projectile.StartRotation = initRotation;
        projectile.PositionGenerator = positionGeneratorDelegate;
        projectile.RotationGenerator = rotationGeneratorDelegate;
        return projectile;
    }

    #region 攻击

    [Export] public float Damage { get; set; } = 1;

    private void OnHit(HurtBox hurtbox)
    {
        QueueFree();
    }

    #endregion

    #region 运动轨迹

    private float ElaspedTime { get; set; }

    [Export] public required Vector2 StartPosition { get; set; }

    [Export] public required float StartRotation { get; set; }

    public delegate Vector2 PositionGeneratorDelegate(Vector2 startPosition, Vector2 currentPosition, float elaspedTime,
        double delta, Projectile projectile);

    public delegate float RotationGeneratorDelegate(float startRotation, float currentRotation, float elaspedTime,
        double delta, Projectile projectile);

    public required PositionGeneratorDelegate PositionGenerator { get; set; }
    public required RotationGeneratorDelegate RotationGenerator { get; set; }

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        base._Ready();
        VisibleOnScreenNotifier.ScreenExited += QueueFree;
        HitBox.Hit += OnHit;

        ElaspedTime = 0;
        Position = StartPosition;
        Rotation = StartRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        ElaspedTime += (float)delta;

        Position = PositionGenerator(StartPosition, Position, ElaspedTime, delta, this);
        Rotation = RotationGenerator(StartRotation, Rotation, ElaspedTime, delta, this);
    }

    #endregion

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public VisibleOnScreenNotifier2D VisibleOnScreenNotifier { get; set; } = null!;

    [Export] public HitBox HitBox { get; set; } = null!;

    #endregion
}