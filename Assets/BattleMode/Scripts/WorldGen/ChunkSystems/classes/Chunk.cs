using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public List<Vector3> Verticies = new List<Vector3>();
    public List<int> Indices = new List<int>();
    public List<Vector3> norm = new List<Vector3>();
    public List<Vector2> uv = new List<Vector2>();

    public World.ChunkPoint Position;
    public GameObject chunk;
    public Dictionary<World.LODLEVELS, Mesh> savedMeshed = new Dictionary<World.LODLEVELS, Mesh>(4);


    public World.LODLEVELS LOD;


    public WorldTypes worldType;

    public Chunk(World.ChunkPoint position)
    {
        this.Position = position;

    }
    public int PosToIndex(int x, int y)
    {

        var index = x * (World.chunkSize + 1) + y * (World.chunkSize + 1);
        return index;
        //return x + ChunkSize * (y + ChunkSize * z);
    }
    public void GenerateObject(Material material, Vector3 origin, Transform parent, int chunkRes, World.LODLEVELS lod)
    {

        var mesh = new Mesh
        {
            vertices = Verticies.ToArray(),
            triangles = Indices.ToArray(),
            uv = uv.ToArray()
        };
        mesh.name = lod.ToString();

        mesh.RecalculateBounds();


        //mesh.normals = norm.ToArray();
        mesh.RecalculateNormals();

        var go = new GameObject("TerrainChunk");
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var collider = go.AddComponent<MeshCollider>();
        mr.material = material;
        mf.mesh = mesh;
        collider.sharedMesh = mesh;
        go.transform.position = origin;
        go.transform.parent = parent;
        chunk = go;

        savedMeshed[lod] = mesh;
        LOD = lod;
    }

    public bool hasMesh(World.LODLEVELS lod)
    {
        return savedMeshed.ContainsKey(lod);
    }

    public void updateMesh(World.LODLEVELS lod)
    {
        MeshFilter mf = chunk.GetComponent<MeshFilter>();
        if (savedMeshed.ContainsKey(lod) && !World.editing)
        {
            mf.mesh = savedMeshed[lod];
        }
        else
        {

            Mesh mesh = new Mesh();
            mesh.name = lod.ToString();
            mesh.vertices = Verticies.ToArray();
            mesh.triangles = Indices.ToArray();
            mesh.RecalculateBounds();
            //mesh.normals = norm.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = uv.ToArray();
            mf.mesh = mesh;
            savedMeshed[lod] =mesh;
            LOD = lod;
        }
    }
    public void Dispose()
    {
        Verticies.Clear();
        Indices.Clear();
    }

}
