using Godot;

public partial class Bg : Node2D
{
    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public Parallax2D BGParallax2D { get; set; } = null!;

    #endregion

    [Export] public float MovingSpeed { get; set; } = -5000;
    [Export] public float MovingAcceleration { get; set; } = -15000;

    private bool IsMoving { get; set; }

    public void StartMove()
    {
        IsMoving = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsMoving)
        {
            BGParallax2D.Autoscroll = BGParallax2D.Autoscroll with
            {
                X = BGParallax2D.Autoscroll.X + MovingSpeed * (float)delta
            };
            MovingSpeed += MovingAcceleration * (float)delta;
        }
    }
}