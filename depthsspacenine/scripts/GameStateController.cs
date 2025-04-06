using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameStateController : Node
{
    [Export]
    public Resource ResourceData { get; set; }

    [Export]
    public Resource[] Constellations { get; set; }

    [Export]
    public float DetectionProgress { get; set; }

    [Export]
    public Node3D StarLayer { get; set; }

    [Export]
    public Node3D TargetLayer { get; set; }
    [Export]
    public Material TargetDisplayMaterial { get; set; }

    [Export]
    public PackedScene StarSmall { get; set; }
    [Export]
    public PackedScene StarMedium { get; set; }
    [Export]
    public PackedScene StarLarge { get; set; }

    [Export]
    public float ConstellationScalingBackground { get; set; }
    [Export]
    public float ConstellationScalingTarget { get; set; } 

    [Export]
    public float AngleTolerance { get; set; }

    public float CurrentLevelAngle { get; set; }

    public float TransitionTimer { get; set; }
    public bool RequireNewMap => CurrentConstellation == null && TransitionTimer <= 0;
    public Constellation CurrentConstellation { get; set; }

    public override void _Process(double delta)
    {
        DetectionProgress += (float)(delta * 0.1f);
        if (TransitionTimer > 0)
        {
            TargetLayer.Visible = TransitionTimer > 2;
            TransitionTimer -= (float)delta;
            if (TransitionTimer < 0)
            {
                CurrentConstellation = null;
            }
        }
        else
        {
            TargetLayer.Visible = true;
        }

        if (RequireNewMap && Constellations.Length > 0)
        {
            var nextConstellation = Constellations[GD.Randi() % Constellations.Length];
            var dataFile = FileAccess.Open(nextConstellation.ResourcePath, FileAccess.ModeFlags.Read);
            var result = Json.ParseString(dataFile.GetAsText());
            
            var dict = result.AsGodotDictionary();

            if (dict != null)
            {
                CurrentConstellation = LoadConstellation(dict);
                ApplyConstellation(CurrentConstellation);
                GD.Print($"Updated constellation, new constellation: {CurrentConstellation.Name} @ {CurrentLevelAngle}°");
                DetectionProgress = 0;
            }
        }    
    }

    private void ApplyLine(Line line)
    {
        var pathItem = new Path3D();
        var csgPolygon = new CsgPolygon3D();
        TargetLayer.AddChild(pathItem);
        TargetLayer.AddChild(csgPolygon);
        csgPolygon.Mode = CsgPolygon3D.ModeEnum.Path;
        csgPolygon.PathNode = pathItem.GetPath();
        csgPolygon.Polygon = new Vector2[]
        {
            new Vector2(-0.01f, -0.01f),
            new Vector2(-0.01f, 0.01f),
            new Vector2(0.01f, 0.01f),
            new Vector2(0.01f, -0.01f),
        };
        csgPolygon.PathIntervalType = CsgPolygon3D.PathIntervalTypeEnum.Distance;
        csgPolygon.PathInterval = 1;
        csgPolygon.PathSimplifyAngle = 0;
        csgPolygon.PathRotation = CsgPolygon3D.PathRotationEnum.Path;
        csgPolygon.PathLocal = true;
        csgPolygon.PathContinuousU = true;
        csgPolygon.PathUDistance = 1;
        csgPolygon.PathJoined = false;
        csgPolygon.SmoothFaces = false;
        csgPolygon.Material = TargetDisplayMaterial;

        var start = new Vector3(line.Start.X, 0, line.Start.Y) * ConstellationScalingTarget;
        var end = new Vector3(line.End.X, 0, line.End.Y) * ConstellationScalingTarget;

        pathItem.Curve = new Curve3D();
        pathItem.Curve.AddPoint(start);
        pathItem.Curve.AddPoint(end);
    }

    private void ApplyConstellation(Constellation data)
    {
        var starLayerChildren = StarLayer.GetChildren();
        foreach (var oldStar in starLayerChildren)
        {
            oldStar.QueueFree();
        }
        var targetLayerChildren = TargetLayer.GetChildren();
        foreach(var oldTargetLayerItem in targetLayerChildren)
        {
            oldTargetLayerItem.QueueFree();
        }

        foreach (var starData in data.Stars)
        {
            Node3D newStar;
            switch (starData.Size?.ToLower())
            {
                case "l":
                    newStar = StarLarge.Instantiate<Node3D>();
                    break;
                case "m":
                    newStar = StarMedium.Instantiate<Node3D>();
                    break;
                default:
                    newStar = StarSmall.Instantiate<Node3D>();
                    break;
            }
            var starPos = starData.Position * ConstellationScalingBackground;
            newStar.Position = new Vector3(starPos.X, starPos.Y, 0);
            newStar.Name = $"Star_{starData.ID}";
            StarLayer.AddChild(newStar);
        }
        var randomStarsSmall = GD.RandRange(8, 12);
        for (int i = 0; i < randomStarsSmall; i++)
        {
            Node3D newStar = StarSmall.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            var y = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarS_{i}";
            StarLayer.AddChild(newStar);
        }
        for (int i = 0; i < randomStarsSmall; i++)
        {
            Node3D newStar = StarSmall.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            var y = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarS_{i}";
            StarLayer.AddChild(newStar);
        }
        var randomStarsMedium = GD.RandRange(6, 8);
        for (int i = 0; i < randomStarsMedium; i++)
        {
            Node3D newStar = StarMedium.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            var y = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarM_{i}";
            StarLayer.AddChild(newStar);
        }
        for (int i = 0; i < randomStarsMedium; i++)
        {
            Node3D newStar = StarMedium.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            var y = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarM_{i}";
            StarLayer.AddChild(newStar);
        }
        var randomStarsLarge = GD.RandRange(4, 6);
        for (int i = 0; i < randomStarsLarge; i++)
        {
            Node3D newStar = StarLarge.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            var y = (float)GD.RandRange(-1, 1) * ConstellationScalingBackground;
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarL_{i}";
            StarLayer.AddChild(newStar);
        }
        for (int i = 0; i < randomStarsLarge; i++)
        {
            Node3D newStar = StarLarge.Instantiate<Node3D>();
            var x = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            var y = (float)GD.RandRange(-1, 1) * (ConstellationScalingBackground * 0.5f);
            newStar.Position = new Vector3(x, y, 0);
            newStar.Name = $"RandomStarL_{i}";
            StarLayer.AddChild(newStar);
        }

        foreach (var lineData in data.Lines)
        {
            ApplyLine(lineData);
        }

        CurrentLevelAngle = GD.RandRange(0, 360);
        StarLayer.RotationDegrees = new Vector3(0, 0, CurrentLevelAngle);
        //StarLayer.RotateZ(Mathf.DegToRad(CurrentLevelAngle));
    }

    private Constellation LoadConstellation(Dictionary dict)
    {
        var constellation = new Constellation
        {
            Name = dict["name_sci"].AsString()
        };

        List<StarData> stars = new List<StarData>();
        var coordsArray = dict["coordinates"].AsGodotArray();
        foreach (var coordinate in coordsArray)
        {
            var star = coordinate.AsGodotDictionary();
            stars.Add(new StarData
            {
                ID = star["id"].AsString(),
                Position = new Vector2(
                    (star["x"].AsInt32() - 500) * 0.002f,
                    (star["y"].AsInt32() - 500) * 0.002f
                ),
                Size = star["size"].AsString()
            });
        } 
        constellation.Stars = stars.ToArray();

        List<Line> lines = new List<Line>();
        var linesArray = dict["lines"].AsGodotArray();
        foreach (var line in linesArray)
        {
            var lineData = line.AsGodotDictionary();
            lines.Add(new Line()
            {
                Start = stars.First(s => s.ID == lineData["from"].AsString()).Position,
                End = stars.First(s => s.ID == lineData["to"].AsString()).Position
            });
        }
        constellation.Lines = lines.ToArray();
        return constellation;
    }

    public bool IsCloseEnough(float angle)
    {
        angle += 360;
        angle %= 360;
        var upperBound = angle + AngleTolerance;
        var lowerBound = angle - AngleTolerance;
        return CurrentLevelAngle >= lowerBound && CurrentLevelAngle <= upperBound;
    }


    public class Constellation
    {
        public string Name { get; set; }
        public StarData[] Stars { get; set; }
        public Line[] Lines { get; set; }
    }

    public class StarData
    {
        public string ID { get; set; }
        public Vector2 Position { get; set; }
        public string Size { get; set; }
    }

    public class Line
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
    }
}
