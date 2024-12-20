using Godot;
using Thunder.Autoloads;
using Thunder.Constants;

namespace Thunder.UI;

public partial class TitleScreen : Control
{
    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public CoinCount CoinCount { get; set; } = null!;

    #endregion

    public override void _Ready()
    {
        base._Ready();
        AutoloadManager.SoundManager.SetupUISounds(this);
        AutoloadManager.SoundManager.PlayBGM(ResourceLoader.Load<AudioStream>(BGMPaths.Master));
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (@event.IsActionPressed("pause"))
            QuitGame();
        if (@event.IsActionPressed("confirm") && CoinCount.Count > 0)
        {
            CoinCount.Count--;
            StartGame();
        }

        if (@event.IsActionPressed("coin"))
            // todo 添加投币音效
            CoinCount.Count++;
    }

    private void QuitGame()
    {
        GetTree().Quit();
    }

    private void StartGame()
    {
        AutoloadManager.SceneTranslation.ChangeSceneToFileWithPause(ScenePaths.Animations.EnterGame);
    }
}