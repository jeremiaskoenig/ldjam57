using Godot;
using System;

public partial class WinDisplay : Label3D
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
		Text = initialText.Replace("{time}", GameState.TimeInCurrentRun.ToString("0.00"));
	}
}
