
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Vector3[] Verticies;
    public int[] Indices;
    public Vector3[] norm;
    public Vector2[] uv;
    public float[] noisemap;

    public World.ChunkPoint Position;
    public GameObject chunk;
    public Dictionary<World.LODLEVELS, Mesh> savedMeshed = new Dictionary<World.LODLEVELS, Mesh>();


    public World.LODLEVELS LOD;


    public WorldTypes worldType;


    public MeshCollider collider;
    MeshFilter mf;
    MeshRenderer mr;

    TreePlacement tr;

    Material baseS;
    WorldTypes typeOfWorld;
    public int chunkRes;


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
    public void GenerateObject(float[] noisemap,WorldTypes typeOfWorld,Material baseS, Vector3 origin, Transform parent, int chunkRes, World.LODLEVELS lod)
    {

        var mesh = new Mesh
        {
            vertices = Verticies,
            triangles = Indices,
            normals = norm,
            uv = this.uv
        };
        mesh.name = lod.ToString();
        //mesh.RecalculateBounds();
        //mesh.normals = norm.ToArray();
        this.baseS = baseS;
        this.typeOfWorld = typeOfWorld;
        this.chunkRes = chunkRes;
        chunk = new GameObject("TerrainChunk: "+Position);
        mf = chunk.AddComponent<MeshFilter>();
        mr = chunk.AddComponent<MeshRenderer>();
        collider = chunk.AddComponent<MeshCollider>();
        collider.cookingOptions = MeshColliderCookingOptions.None;
        this.noisemap = noisemap;
        //mr.material = GenerateMaterial(baseS, typeOfWorld, noisemap, chunkRes);
        mr.material = baseS;
        mf.sharedMesh = mesh;
        collider.cookingOptions = MeshColliderCookingOptions.UseFastMidphase;
        chunk.transform.position = origin;
        chunk.transform.parent = parent;
        tr = chunk.AddComponent<TreePlacement>();

        savedMeshed[lod] = mesh;
        LOD = lod;
        if(lod == World.LODLEVELS.LOD1) collider.sharedMesh = mesh;
        if (lod <= World.LODLEVELS.LOD1)
        updateMesh(lod);
    }

    public bool hasMesh(World.LODLEVELS lod)
    {
        return savedMeshed.ContainsKey(lod);
    }

    public void updateMesh(World.LODLEVELS lod)
    {
        if (savedMeshed.ContainsKey(lod))
        {
            mf.sharedMesh = savedMeshed[lod];
            //collider.sharedMesh = savedMeshed[lod];
            //mr.material = GenerateMaterial(baseS, typeOfWorld, noisemap, chunkRes);
            LOD = lod;

        }
        else
        {

            Mesh mesh = new Mesh
            {
                name = lod.ToString(),
                vertices = Verticies,
                triangles = Indices,
                normals = norm,
                uv = this.uv
            };
            //mesh.normals = norm.ToArray();
            //mr.material = GenerateMaterial(baseS, typeOfWorld, noisemap, chunkRes);
            mf.sharedMesh = mesh;
            savedMeshed[lod] = mesh;
            LOD = lod;
            
        }
        if (lod < World.LODLEVELS.LOD1)
        {
            tr.chunkplaceTree(chunk.transform.position + savedMeshed[lod].bounds.extents, 0);
            tr.showTrees();

        }
        else
        {
            tr.HideTrees();
        }
    }

}
