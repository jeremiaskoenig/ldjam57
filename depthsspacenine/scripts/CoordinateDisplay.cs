using Godot;

public partial class CoordinateDisplay : Label3D
{
    [Export]
    public GameStateController GameState { get; set; }

    private float offset = 0;

    public override void _Process(double delta)
    {
        offset += (float)delta;

        if (offset > 0.2f) 
        {
            UpdateText();
            offset = 0;
        }
    }

    private static readonly char[] possibleRandomChars = new char[]
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    };

    private void UpdateText()
    {
        string text;
        if (GameState.TransitionTimer > 0)
        {
            if (GameState.TransitionTimer > 2)
            {
                text = $"Target system acquired!\n\nTarget: {GameState.CurrentConstellation.Name}";
            }
            else
            {
                text = "";
            }
        }
        else
        {
            string coords = "";
            for (int i = 0; i < 3; i++)
                coords += possibleRandomChars[GD.Randi() % possibleRandomChars.Length];
            coords += ":";
            for (int i = 0; i < 4; i++)
                coords += possibleRandomChars[GD.Randi() % possibleRandomChars.Length];
            coords += ":";
            for (int i = 0; i < 4; i++)
                coords += possibleRandomChars[GD.Randi() % possibleRandomChars.Length];
            coords += ":";
            for (int i = 0; i < 3; i++)
                coords += possibleRandomChars[GD.Randi() % possibleRandomChars.Length];

            text = $"Acquiring target coordinates\n\n{coords}";
        }

        Text = text;
    }
}
