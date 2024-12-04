using System.Threading.Tasks;
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
        AirPlane.OutOfScreen += async () => { await EnterGame(); };

        await this.DelayAsync(0.5f);
        AirPlane.TakeOff();
    }

    private async Task EnterGame()
    {
        var tween = CreateTween();
        tween.TweenProperty(Mask, "color:a", 1, 2);
        await ToSignal(tween, Tween.SignalName.Finished);
        AutoloadManager.SceneTranslation.ChangeSceneToFileAsync(ScenePaths.TestWorld);
    }

    public override async void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("primary")) await EnterGame();
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public AirPlane AirPlane { get; set; } = null!;

    [Export] public Bg Bg { get; set; } = null!;
    [Export] public ColorRect Mask { get; set; } = null!;

    #endregion
}