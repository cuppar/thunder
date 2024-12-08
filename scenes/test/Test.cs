using Godot;
using Thunder.Enemys;
#if IMGUI
using ImGuiGodot;

// using ImGuiNET;
#endif

namespace Thunder;

public partial class Test : Node2D
{
    public override void _Ready()
    {
        base._Ready();
#if IMGUI
        ImGuiGD.Connect(OnImGuiLayout);
#endif
    }

    #region debug

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var target = GetNode<Ball>("Debug/Ball");
        var player = GetNode<Player>("Player");
        player.SetTarget(target);
    }

    #endregion

#if IMGUI
    private void OnImGuiLayout()
    {
        // ImGui.Begin("ImGui on Godot 4");
        // ImGui.Text("hello world");
        // ImGui.End();
    }
#endif
}