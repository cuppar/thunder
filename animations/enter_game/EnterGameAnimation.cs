using Godot;
using Thunder.Globals.Extensions;

namespace Thunder.Animations.EnterGame;

public partial class EnterGameAnimation : Node2D
{
    public override async void _Ready()
    {
        base._Ready();
        await this.DelayAsync(1);
        AirPlane.TakeOff();
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public AirPlane AirPlane { get; set; } = null!;

    #endregion
}