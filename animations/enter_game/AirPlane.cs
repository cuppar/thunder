using Godot;

namespace Thunder.Animations.EnterGame;

public partial class AirPlane : Node2D
{
    #region Delegates

    [Signal]
    public delegate void OutOfScreenEventHandler();

    [Signal]
    public delegate void StartMoveEventHandler();

    #endregion

    private bool IsMoving { get; set; }
    [Export] public float MovingSpeed { get; set; } = 50;
    [Export] public float MovingAcceleration { get; set; } = 20;

    public async void TakeOff()
    {
        AnimationPlayer.Play("ready");
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        AnimationPlayer.Play("fly");
        EmitSignal(SignalName.StartMove);
    }

    public override void _Ready()
    {
        base._Ready();
        StartMove += () => IsMoving = true;
        VisibleOnScreenNotifier.ScreenExited += () => EmitSignal(SignalName.OutOfScreen);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsMoving)
        {
            Position = Position with
            {
                X = Position.X + MovingSpeed * (float)delta
            };
            MovingSpeed += MovingAcceleration * (float)delta;
        }
    }


    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public AnimationPlayer AnimationPlayer { get; set; } = null!;

    [Export] public VisibleOnScreenNotifier2D VisibleOnScreenNotifier { get; set; } = null!;

    #endregion
}