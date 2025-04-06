using Godot;
using System;
using System.Linq;

public partial class DetectionDisplay : Label3D
{
    [Export]
    public GameStateController GameState { get; set; }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        string text;
        if (GameState.TransitionTimer > 0)
        {
            if (GameState.TransitionTimer > 2)
            {
                text = $"Jump initiated!\n\nJump in {(GameState.TransitionTimer - 2):0.00}s";
            }
            else
            {
                text = "";
            }
        }
        else
        {
            var progressArrows = (int)(GameState.DetectionProgress * 23);
            string progress = String.Empty.PadRight(progressArrows, '>');
            if (progress.All(c => c == '>') && progress.Length > 23)
                progress = progress.Substring(0, 23);
            else
                progress = progress.PadRight(progress.Length + ((23 - progressArrows) * 2), ' ');
            text = $"Glorpixian Detection Probe\n\n[{progress}]";
        }

        Text = text;
    }
}
