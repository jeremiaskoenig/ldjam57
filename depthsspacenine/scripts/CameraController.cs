using Godot;
using System;

public partial class CameraController : Camera3D
{
    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey keyInput)
        {
            if (keyInput.Keycode == Key.Q)
            {
                RotateZ(Mathf.DegToRad(-5));
            }
            else if (keyInput.Keycode == Key.E)
            {
                RotateZ(Mathf.DegToRad(5));
            }
        }
    }
}
