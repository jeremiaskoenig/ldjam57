using Godot;

public partial class CameraController : Camera3D
{
    [Export]
    public GameStateController GameState { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        if (GameState.TransitionTimer > 0)
            return;

        if (Input.IsKeyPressed(Key.Q))
        {
            RotateZ(Mathf.DegToRad(-2));
        }
        else if (Input.IsKeyPressed(Key.E))
        {
            RotateZ(Mathf.DegToRad(2));
        }
    }

    float inputCooldown = 0.5f;

    public override void _Process(double delta)
    {
        if (GameState.TransitionTimer > 0)
            return;

        inputCooldown -= (float)delta;
        if (inputCooldown <= 0 && GameState.IsCloseEnough(RotationDegrees.Z))
        {
            GameState.TransitionTimer = 5;
        }
    }

    public override void _Input(InputEvent e)
    {
        if (GameState.TransitionTimer > 0)
            return;

        inputCooldown = 0.5f;
    }
}
