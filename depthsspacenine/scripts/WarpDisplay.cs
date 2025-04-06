using Godot;
using System;

public partial class WarpDisplay : Node3D
{
    [Export]
    public GameStateController GameState { get; set; }

    public override void _Process(double delta)
    {
        Visible = GameState.TransitionTimer > 0 && GameState.TransitionTimer <= 2;
    }
}
