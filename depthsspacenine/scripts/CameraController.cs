using Godot;

public partial class CameraController : Camera3D
{
    [Export]
    public GameStateController GameState { get; set; }

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
        }
        else if (Input.IsKeyPressed(Key.E))
        {
            RotateZ(Mathf.DegToRad(speed));
        }
    }

    float inputCooldown = 0.5f;

    public override void _Process(double delta)
    {
        if (GameState.TransitionTimer > 0)
            return;

        inputCooldown -= (float)delta;
        if (inputCooldown <= 0 && GameState.IsCloseEnough(CurrentAngle))
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
