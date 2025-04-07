using Godot;
using System;

public partial class LevelProgressDisplay : Label3D
{
    [Export]
    public GameStateController GameState { get; set; }

    private string initialText;

    public override void _Ready()
    {
        initialText = Text;
    }

    public override void _Process(double delta)
    {
        Visible = GameState.CurrentLevel > 0;
        Text = initialText.Replace("{now}", GameState.CurrentLevel.ToString())
                          .Replace("{max}", GameState.LevelCount.ToString());
    }
}
