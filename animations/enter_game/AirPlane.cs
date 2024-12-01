using Godot;

namespace Thunder.Animations.EnterGame;

public partial class AirPlane : Node2D
{
    public async void TakeOff()
    {
        AnimationPlayer.Play("ready");
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        AnimationPlayer.Play("fly");
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public AnimationPlayer AnimationPlayer { get; set; } = null!;

    #endregion
}