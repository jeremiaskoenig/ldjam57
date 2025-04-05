using Godot;
using Godot.Collections;
using System;

public partial class GameStateController : Node
{
    [Export]
    public Resource ResourceData { get; set; }

    [Export]
    public float DetectionProgress { get; set; }

    [Export]
    public bool RequireNewMap { get; set; }

    public override void _Process(double delta)
    {
        DetectionProgress += (float)(delta * 0.1f);

        if (RequireNewMap)
        {
            var dataFile = FileAccess.Open(ResourceData.ResourcePath, FileAccess.ModeFlags.Read);
            var result = Json.ParseString(dataFile.GetAsText());
            
            var dict = result.AsGodotDictionary();

            if (dict != null)
            {
                
            }
        }    
    }
}
