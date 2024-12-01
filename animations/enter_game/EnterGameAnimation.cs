using Godot;
using Thunder.Autoloads;
using Thunder.Constants;
using Thunder.Globals.Extensions;

namespace Thunder.Animations.EnterGame;

public partial class EnterGameAnimation : Node2D
{
    public override async void _Ready()
    {
        base._Ready();
        AirPlane.StartMove += () => { Bg.StartMove(); };
        AirPlane.OutOfScreen += async () =>
        {
            var tween = CreateTween();
            tween.TweenProperty(Mask, "color:a", 1, 2);
            await ToSignal(tween, Tween.SignalName.Finished);
            AutoloadManager.SceneTranslation.ChangeSceneToFileAsync(ScenePaths.TestWorld);
        };

        await this.DelayAsync(0.5f);
        AirPlane.TakeOff();
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public AirPlane AirPlane { get; set; } = null!;

    [Export] public Bg Bg { get; set; } = null!;
    [Export] public ColorRect Mask { get; set; } = null!;

    #endregion
}