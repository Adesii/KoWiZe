using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class chunkGenerator : MonoBehaviour
{
    //ChunkSettings
    public static int height = 32;
    public static int width = 32;


    public Material tempMaterial;

    public enum LODLevels
    {
        LOD0,
        LOD1,
        LOD2,
        LOD3
    }

    //world Settings
    public int numberOfChunks;

    [Header("Generation Settings")]
    public string seed = "Toyota";
    public WorldTypes type;
    public float noiseScale;
    public float noiseAmplitut;

    //generic Variables
    SimplexNoiseGenerator noiseGen;
    List<chunkObject> chunksList = new List<chunkObject>();
    List<chunkObject> activeChunks;


    chunkObject debugDrawChunk;

    // Start is called before the first frame update
    void Start()
    {
        noiseGen = new SimplexNoiseGenerator(seed);
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {
        for (int x = -numberOfChunks; x < numberOfChunks; x++)
        {
            for (int z = -numberOfChunks; z < numberOfChunks; z++)
            {
                chunkObject chunkGameObject = new chunkObject(new GameObject("Chunk_" + x + "_" + z), new Vector3Int(x * width, 0, z * width), transform, tempMaterial);
                chunksList.Add(chunkGameObject);
                
                
            }
        }
        foreach (var item in chunksList)
        {
            GenerateMeshes(item, LODLevels.LOD2);
        }
    }
    private void OnDrawGizmos()
    {
        if (debugDrawChunk !=null)
        {
            for (int i = 0; i < debugDrawChunk.verts.Length; i++)
            {
                Gizmos.DrawSphere(debugDrawChunk.verts[i]+debugDrawChunk.position, 0.1f);
            }
            for (int i = 0; i < chunksList.Count; i++)
            {
                Gizmos.DrawWireCube(chunksList[i].position, new Vector3(width, height, width));
            }
        }
        
    }
    void GenerateMeshes(chunkObject chunk,LODLevels quality)
    {
        float resolution = 0;
        switch (quality)
        {
            case LODLevels.LOD0:
                resolution = 128f;
                break;
            case LODLevels.LOD1:
                resolution = 64f;
                break;
            case LODLevels.LOD2:
                resolution = 16f;
                break;
            case LODLevels.LOD3:
                resolution = 8f;
                break;
        }
        Mesh tempMesh = new Mesh();
        Vector3[] tempVerts = new Vector3[width * width];
        int[] tris = new int[width * width * 6];
        for (int x = 0; x < resolution+1; x++)
        {
            for (int z = 0; z < resolution+1; z++)
            {
                float heightNoise = noiseGen.coherentNoise(((x / resolution) * width) + (chunk.position.x), 0f, ((z / resolution) * width) + (chunk.position.z), 10,100,4);
                tempVerts[x+(int)(resolution+1f)*z]=new Vector3(((x / resolution) * width)-(width/2),heightNoise+1f, ((z / resolution) * width)-(width / 2));
            }
        }
        int trisIndex = 0;
        int vert = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int i = 0; i < resolution; i++)
            {

                tris[trisIndex + 2] = vert;
                tris[trisIndex + 1] = (int)(vert + resolution + 1 + 1);
                tris[trisIndex + 0] = (int)(vert + resolution + 1);
                tris[trisIndex + 5] = vert;
                tris[trisIndex + 4] = (vert + 1);
                tris[trisIndex + 3] = (int)(vert + resolution + 1 + 1);

                vert++;
                trisIndex += 6;
            }
            vert++;
        }
        
        tempMesh.bounds = new Bounds(chunk.position, new Vector3(width, height, width));
        tempMesh.vertices = tempVerts;
        tempMesh.triangles = tris;
        chunk.setMesh(tempMesh);
        chunk.verts = tempVerts;
        debugDrawChunk = chunk;
    }

    public class chunkObject
    {
        public GameObject chunkGameObject;
        public Vector3Int position;
        public MeshFilter meshFil;
        public MeshRenderer meshRen;
        public Mesh mesh;
        public Vector3[] verts; 


        public chunkObject(GameObject chunk,Vector3Int position,Transform parent,Material tempMaterial)
        {
            chunkGameObject = chunk;
            this.position = position;
            meshFil=chunkGameObject.AddComponent<MeshFilter>();
            meshRen = chunkGameObject.AddComponent<MeshRenderer>();
            chunkGameObject.transform.parent = parent;
            chunkGameObject.transform.position = position;
            meshRen.material = tempMaterial;
        }
        public void setMesh(Mesh newMesh)
        {
            newMesh.RecalculateNormals();
            
            mesh = newMesh;
            meshFil.mesh = newMesh;
        }
    }


}
