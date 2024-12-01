using System;
using System.Linq;
using Godot;
using Thunder.Animations.EnterGame;
using Thunder.Autoloads;

namespace Thunder.UI;

public partial class HUD : CanvasLayer
{
    private static readonly Type[] ScenesNeedHideHUD = { typeof(TitleScreen), typeof(EnterGameAnimation) };

    private void HideHUDInSomeScene()
    {
        AutoloadManager.SceneTranslation.AfterSceneChanged += (_, newScene) =>
        {
            Show();
            if (ScenesNeedHideHUD.Any(scene => newScene.GetType() == scene)) Hide();
        };
    }

    public override void _Ready()
    {
        base._Ready();
        Hide();
        HideHUDInSomeScene();
    }
}