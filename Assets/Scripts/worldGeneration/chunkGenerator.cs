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
    public string seed;
    public WorldTypes worldType;
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
        int randomOffset = UnityEngine.Random.Range(-100, 100);
        for (int x = 0; x < numberOfChunks; x++)
        {
            for (int z = 0; z < numberOfChunks; z++)
            {
                chunkObject chunkGameObject = new chunkObject(new GameObject("Chunk_" + x + "_" + z), new Vector3Int((int)(x * width), 0, (int)(z * width)), transform, tempMaterial, randomOffset);
                chunksList.Add(chunkGameObject);
                
                
            }
        }
        foreach (var item in chunksList)
        {
            GenerateMeshes(item, LODLevels.LOD0);
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
                resolution = 128;
                break;
            case LODLevels.LOD1:
                resolution = 64;
                break;
            case LODLevels.LOD2:
                resolution = 16;
                break;
            case LODLevels.LOD3:
                resolution = 8;
                break;
        }
        Mesh tempMesh = new Mesh();
        Vector3[] tempVerts = new Vector3[(int)((resolution+1) * (resolution+1))];
        int[] tris = new int[(int)((resolution+1) * (resolution+1) * 6)];
        for (int x = 0; x < resolution+1; x++)
        {
            for (int z = 0; z < resolution+1; z++)
            {
                float posx= ((x / resolution) * width) +(chunk.position.x);
                float posz = ((z / resolution) * width) + (chunk.position.z);
                float heightNoise = noiseGen.coherentNoise(posx*0.25f, 0f, posz*0.25f, 2,50,1);
                float multiplyNoise = noiseGen.coherentNoise(posx*0.5f, 0f, posz * 0.5f,2,100,4);
                if (multiplyNoise * 10 > heightNoise) {

                    heightNoise = Mathf.Lerp(heightNoise, (multiplyNoise * 10f), Mathf.Clamp01(multiplyNoise));
                    if (Mathf.Clamp01(heightNoise) > 0.05f)
                    {
                        multiplyNoise = noiseGen.coherentNoise(posx*0.125f, 0f, posz*0.125f, 3, 10, 4);
                        heightNoise += Mathf.Lerp(heightNoise, multiplyNoise, Mathf.Clamp01(heightNoise));
                    }
                }
                //Debug.Log(tempVerts.Length+"/"+(x + (resolution + 1) * z));
                tempVerts[x+(int)(resolution+1f)*z]=
                    new Vector3(((x / resolution) * width)-(width/2f),heightNoise,
                    ((z / resolution) * width)-(width / 2));
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
        public MeshCollider meshCol;
        public Mesh mesh;
        public Vector3[] verts; 


        public chunkObject(GameObject chunk,Vector3Int position,Transform parent,Material tempMaterial,int randomOffset)
        {
            chunkGameObject = chunk;
            Vector3Int newPos = new Vector3Int(position.x + randomOffset, 0, position.z + randomOffset);
            this.position = newPos;
            meshFil=chunkGameObject.AddComponent<MeshFilter>();
            meshRen = chunkGameObject.AddComponent<MeshRenderer>();
            meshCol = chunkGameObject.AddComponent<MeshCollider>();
            chunkGameObject.transform.parent = parent;
            chunkGameObject.transform.position = position;
            meshRen.material = tempMaterial;
        }
        public void setMesh(Mesh newMesh)
        {
            newMesh.RecalculateNormals();
            meshCol.sharedMesh = newMesh;
            mesh = newMesh;
            meshFil.mesh = newMesh;

        }
    }

    
}
