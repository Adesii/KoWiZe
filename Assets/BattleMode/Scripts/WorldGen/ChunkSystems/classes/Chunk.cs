using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public List<Vector3> Verticies = new List<Vector3>();
    public List<int> Indices = new List<int>();
    public List<Vector3> norm = new List<Vector3>();
    public World.ChunkPoint Position;
    public GameObject chunk;
    public World.LODLEVELS LOD;
    public Dictionary<World.LODLEVELS,Mesh> savedMeshed = new Dictionary<World.LODLEVELS, Mesh>(4);


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
    public GameObject GenerateObject(Material material, Vector3 origin, Transform parent, int chunkRes, World.LODLEVELS lod)
    {
        if (lod != World.LODLEVELS.LOD4)
        {
            if (Verticies.Count == 0)
                return null;

            var mesh = new Mesh();
            mesh.vertices = Verticies.ToArray();
            mesh.triangles = Indices.ToArray();
            mesh.uv = new Vector2[mesh.vertices.Length];

            mesh.RecalculateBounds();


            //mesh.normals = norm.ToArray();
            mesh.RecalculateNormals();
            var go = new GameObject("TerrainChunk");
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            var collider = go.AddComponent<MeshCollider>();
            mr.material = material;
            mf.sharedMesh = mesh;
            collider.sharedMesh = mesh;
            go.transform.position = origin;
            go.transform.parent = parent;
            //go.transform.Rotate(0f, 0f, -180f);
            chunk = go;
            LOD = lod;
            savedMeshed[lod] = mesh;
            return go;
        }
        return null;
    }

    public bool hasMesh(World.LODLEVELS lod)
    {
        if (savedMeshed.ContainsKey(lod)) return true;
        return false;
    }
    public void removeMesh()
    {
        var mf = chunk.GetComponent<MeshFilter>();
        mf.mesh = null;
        mf.sharedMesh = null;
    }

    public void updateMesh(World.LODLEVELS lod)
    {
        MeshFilter mf = chunk.GetComponent<MeshFilter>();
        if(lod!= World.LODLEVELS.LOD4)
        {

        if (savedMeshed.ContainsKey(lod))
        {
            mf.sharedMesh = savedMeshed[lod];
            LOD = lod;
        }
        else
        {

            Mesh mesh = new Mesh();
            mesh.vertices = Verticies.ToArray();
            mesh.triangles = Indices.ToArray();
            mesh.uv = new Vector2[mesh.vertices.Length];
            mesh.RecalculateBounds();
            //mesh.normals = norm.ToArray();
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;
            LOD = lod;
            savedMeshed[lod] = mesh;
        }
        }
        else
        {
            removeMesh();
        }

    }
    public void Dispose()
    {
        Verticies.Clear();
        Indices.Clear();
    }

}
