using Godot;
using System;
using System.Collections.Generic;

public partial class ProjectorPreview : MeshInstance3D
{
    [Export]
    public Material PreviewMaterial { get; set; }

    [Export]
    public Node3D[] Points { get; set;}
    
    public override void _Ready()
    {
        if (Points.Length != 4)
            return;

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        Vector3 nearCenter = (Points[2].Position + Points[3].Position) / 2.0f;
        Vector3 farCenter = (Points[0].Position + Points[1].Position) / 2.0f;

        verts.Add(Points[0].Position);
        verts.Add(Points[1].Position);
        verts.Add(Points[2].Position);
        verts.Add(Points[3].Position);
        verts.Add(nearCenter);
        verts.Add(farCenter);

        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));
        normals.Add(new Vector3(0, 0, 1));

        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0.5f));
        uvs.Add(new Vector2(1, 0.5f));
        
        addTriangle(3, 0, 5);
        addTriangle(5, 4, 3);
        addTriangle(4, 5, 1);
        addTriangle(1, 2, 4);

        void addTriangle(int a, int b, int c)
        {
            indices.Add(a);
            indices.Add(b);
            indices.Add(c);
        }

        Godot.Collections.Array surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();
        
        var arrMesh = Mesh as ArrayMesh;
        if (arrMesh != null)
        {
            arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
            arrMesh.RegenNormalMaps();
            SetSurfaceOverrideMaterial(0, PreviewMaterial);
        }

        offset = GD.RandRange(0, 360);
    }

    private float offset = 0;

    public override void _Process(double delta)
    {
        offset += (float)delta;
        var alpha = (1 + Mathf.Sin(offset)) / 2.0f;
        alpha = alpha * 0.03f;

        var material = PreviewMaterial as StandardMaterial3D;

        var color = material.AlbedoColor;
        color.A = alpha;
        material.AlbedoColor = color;
    }
}
