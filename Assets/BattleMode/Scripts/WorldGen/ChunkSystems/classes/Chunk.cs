using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public List<Vector3> Verticies = new List<Vector3>();
    public List<int> Indices = new List<int>();
    public World.ChunkPoint Position;
    public float[,] noisemap;


    public WorldTypes worldType;

    public Chunk(World.ChunkPoint position,float[,] noisemap)
    {
        this.Position = position;
        this.noisemap = noisemap;
    }
    public int PosToIndex(int x, int y, int z)
    {

        var index = x * (World.chunkSize + 1) + y * (World.chunkSize + 1) + z;
        return index;
        //return x + ChunkSize * (y + ChunkSize * z);
    }
    public GameObject GenerateObject(Material material, Vector3 origin)
    {
        if (Verticies.Count == 0)
            return null;

        var mesh = new Mesh();
        mesh.vertices = Verticies.ToArray();
        mesh.triangles = Indices.ToArray();
        mesh.uv = new Vector2[mesh.vertices.Length];
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        var go = new GameObject("TerrainChunk");
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var collider = go.AddComponent<MeshCollider>();
        mf.sharedMesh = mesh;
        mr.sharedMaterial = material;
        collider.sharedMesh = mesh;
        go.transform.position = origin;
        //go.transform.Rotate(0f, 0f, -180f);

        return go;
    }
    public void Dispose()
    {
        Verticies.Clear();
        Indices.Clear();
        noisemap = null;
    }
}
