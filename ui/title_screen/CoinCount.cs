using Godot;
using Thunder.Globals.Extensions;

namespace Thunder;

public partial class CoinCount : BoxContainer
{
    #region Count

    private int _count;

    [Export]
    public int Count
    {
        get => _count;
        set => SetCount(value);
    }

    private async void SetCount(int value)
    {
        await this.EnsureReadyAsync();
        if (value < 0)
            value = 0;

        _count = value;
        CountLabel.Text = $"投币数: {Count}";
    }

    #endregion

    private CoinCount()
    {
        SetCount(0);
    }

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public Label CountLabel { get; set; } = null!;

    #endregion
}