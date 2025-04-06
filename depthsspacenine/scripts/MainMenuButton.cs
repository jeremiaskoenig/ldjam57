using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenuButton : MeshInstance3D
{
    private StaticBody3D staticBody;
    private CollisionShape3D collisionShape;
    private Camera3D camera;

    private Vector3 basePosition;

    public event Action ButtonClicked;

    [Export]
    public string Stage { get; set; }

    [Export]
    public string HoverText { get; set; }
    public string CurrentHoverText { get; set; }

    private GameStateController gameState;

    public override void _Ready()
    {
        gameState = FindNodesByType<GameStateController>(GetTree().Root);
        staticBody = FindChild("StaticBody3D") as StaticBody3D;
        collisionShape = staticBody.FindChild("CollisionShape3D") as CollisionShape3D;
        camera = GetViewport().GetCamera3D();
        basePosition = Position;
    }

    public T FindNodesByType<T>(Node root) where T : Node
    {
        var result = new List<T>();
        if (root is T matchingNode)
            result.Add(matchingNode);
        foreach (Node child in root.GetChildren())
            result.Add(FindNodesByType<T>(child));
        return result.Where(n => n != null).FirstOrDefault();
    }

    private float zoomFactor = 1;
    private bool leftState = false;

    public override void _PhysicsProcess(double delta)
    {
        collisionShape.Disabled = gameState.GameStage != Stage;

        bool newLeft = Input.IsMouseButtonPressed(MouseButton.Left);

        if (CheckMouseover())
        {
            CurrentHoverText = HoverText;
            if (zoomFactor > 0)
            {
                zoomFactor -= (float)(delta * 5);
            }

            if (leftState && !newLeft)	
            {
                ButtonClicked?.Invoke();
            }
        }
        else
        {
            CurrentHoverText = null;
            if (zoomFactor < 1)
            {
                zoomFactor += (float)(delta * 5);
            }
        }
        
        Position = basePosition;
        Position += Vector3.Down * zoomFactor;
        leftState = newLeft;
    }

    private bool CheckMouseover()
    {
        var mousePosition = GetViewport().GetMousePosition();
        var rayOrigin = camera.ProjectRayOrigin(mousePosition);
        var rayDirection = camera.ProjectRayNormal(mousePosition);

        var spaceState = GetWorld3D().DirectSpaceState;
        var queryParameters = new PhysicsRayQueryParameters3D
        {
            From = rayOrigin,
            To = rayOrigin + rayDirection * 1000,
            CollideWithBodies = true,
            CollideWithAreas = false
        };

        var result = spaceState.IntersectRay(queryParameters);

        if (result.ContainsKey("collider") && result["collider"].As<StaticBody3D>() == staticBody)
        {
            return true;
        }
        return false;
    }
}
