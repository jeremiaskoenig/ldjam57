using Godot;
using System;

public partial class WarpDisplay : Node3D
{
    [Export]
    public GameStateController GameState { get; set; }

    [Export]
    public CapsuleMesh Capsule { get; set; }
    
    [Export]
    public Curve CapsuleLengthCurve { get; set; }

    public override void _Process(double delta)
    {
        Visible = GameState.TransitionTimer > 0 && GameState.TransitionTimer <= 2;

        if (Visible)
        {
            var t = GameState.TransitionTimer / 2f;

            var capsuleLength = CapsuleLengthCurve.Sample(t);
            Capsule.Height = capsuleLength;
        }
    }
}
