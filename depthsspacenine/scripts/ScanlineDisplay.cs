using Godot;
using System;

public partial class ScanlineDisplay : MeshInstance3D
{
    [Export]
    StandardMaterial3D DisplayMaterial { get; set; }

    float offset = 0f;
    
    public override void _Process(double delta)
    {
        offset += (float)delta * 0.2f;
        var offsetValue = DisplayMaterial.Uv1Offset;
        offsetValue.X = offset;
        DisplayMaterial.Uv1Offset = offsetValue;
    }
}
