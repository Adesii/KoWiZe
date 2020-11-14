using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public Vector3[] Verticies;
    public int[] Indices;
    public Vector3[] norm;
    public Vector2[] uv;

    public World.ChunkPoint Position;
    public GameObject chunk;
    public Dictionary<World.LODLEVELS, Mesh> savedMeshed = new Dictionary<World.LODLEVELS, Mesh>();


    public World.LODLEVELS LOD;


    public WorldTypes worldType;

    TreePlacement tr;

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
            vertices = Verticies,
            triangles = Indices,
            uv = uv
        };
        mesh.name = lod.ToString();
        mesh.MarkDynamic();
        mesh.RecalculateBounds();
        //mesh.normals = norm.ToArray();
        mesh.RecalculateNormals();
        
        var go = new GameObject("TerrainChunk: "+Position);
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var collider = go.AddComponent<MeshCollider>();
        mr.material = material;
        mf.sharedMesh = mesh;
        collider.sharedMesh = mesh;
        go.transform.position = origin;
        go.transform.parent = parent;
        tr = go.AddComponent<TreePlacement>();
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
        MeshCollider collider = chunk.GetComponent<MeshCollider>();
        if (savedMeshed.ContainsKey(lod))
        {
            mf.sharedMesh = savedMeshed[lod];
            
            collider.sharedMesh = savedMeshed[lod];
            LOD = lod;

        }
        else
        {

            Mesh mesh = new Mesh
            {
                name = lod.ToString(),
                vertices = Verticies,
                triangles = Indices,
                uv = uv
            };
            mesh.RecalculateBounds();
            //mesh.normals = norm.ToArray();
            mesh.RecalculateNormals();
            collider.sharedMesh = mesh;
            mf.sharedMesh = mesh;
            savedMeshed[lod] = mesh;
            LOD = lod;
            
        }
        if (lod < World.LODLEVELS.LOD1)
        {
            tr.placeTree(chunk.transform.position + savedMeshed[lod].bounds.extents, 0);
            tr.showTrees();

        }
        else
        {
            tr.HideTrees();
        }
    }

}
