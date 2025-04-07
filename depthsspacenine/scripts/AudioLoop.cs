using Godot;
using System;

public partial class AudioLoop : AudioStreamPlayer
{
    [Export]
    public GameStateController GameState { get; set; }

    public override void _Ready()
    {
        Finished += () => 
        {
            if (GameState.GameStage == "game")
                Play(0);
        };
    }
}
