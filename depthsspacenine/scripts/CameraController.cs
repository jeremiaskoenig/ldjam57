using System;
using Godot;

public partial class CameraController : Camera3D
{
    [Export]
    public GameStateController GameState { get; set; }

    [Export]
    public Curve WarpWobble { get; set; }

    public float CurrentAngle => RotationDegrees.Z;

    public override void _Ready()
    {
        GameState.CameraController = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GameState.TransitionTimer > 0)
            return;

        var speed = 1f;
        if (Input.IsKeyPressed(Key.Shift))
        {
            speed *= 2;
        }

        if (Input.IsKeyPressed(Key.Q))
        {
            RotateZ(Mathf.DegToRad(-speed));
            inputCooldown = 0.5f;
        }
        else if (Input.IsKeyPressed(Key.E))
        {
            RotateZ(Mathf.DegToRad(speed));
            inputCooldown = 0.5f;
        }
    }

    float inputCooldown = 0.5f;

    float? warpRotationStart = null;

    public override void _Process(double delta)
    {
        if (GameState.TransitionTimer > 0)
        {
            if (warpRotationStart == null)
            {
                warpRotationStart = RotationDegrees.Z;
                warpRotationStart += 360;
                warpRotationStart %= 360;
            }
            if (GameState.TransitionTimer > 2)
            {
                var t = Math.Max(0, GameState.TransitionTimer - 3) / 2f;
                var currentAngle = Rotation.Z;
                var startAngle = Mathf.DegToRad(warpRotationStart ?? 0);
                var endAngle = Mathf.DegToRad(GameState.CurrentLevelAngle);
                var currentFrameTargetAngle = Mathf.LerpAngle(endAngle, startAngle, t);
                var rotationDelta = currentFrameTargetAngle - currentAngle;
                RotateZ(rotationDelta);
            }
            else
            {
                warpRotationStart = null;
            }

            if (GameState.TransitionTimer < 0.4)
            {
                var t = GameState.TransitionTimer / 0.4f;
                var value = WarpWobble.Sample(t);

                this.Fov = 37.5f * value;
            }

            return;
        }

        inputCooldown -= (float)delta;
        if (inputCooldown <= 0 && GameState.IsCloseEnough(CurrentAngle))
        {
            GameState.TransitionTimer = 5;
        }
    }
}
