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
    public bool LiveEdit = false;


    public Material tempMaterial;

    public WorldTypes noiseMapGenerator;
    float[,] noiseMap;

    public enum LODLevels
    {
        LOD0,
        LOD1,
        LOD2,
        LOD3
    }

    //world Settings

    //generic Variables
    List<chunkObject> chunksList = new List<chunkObject>();
    List<chunkObject> activeChunks;


    chunkObject debugDrawChunk;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generate());
    }

    // Update is called once per frame
    void Update()
    {
        if (LiveEdit)
        {

        
        foreach (var item in chunksList)
        {
            if (item != null)
                GenerateMeshes(item, LODLevels.LOD2);
        }
        }
    }
    public void RegenerateALL()
    {
        StopAllCoroutines();
        chunksList = new List<chunkObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        for (int x = 0; x < noiseMapGenerator.sizeX; x++)
        {
            for (int z = 0; z < noiseMapGenerator.sizeZ; z++)
            {
                chunkObject chunkGameObject = new chunkObject(new GameObject("Chunk_" + x + "_" + z), new Vector3Int((int)(x * width), 0, (int)(z * width)), transform, tempMaterial);
                chunksList.Add(chunkGameObject);
                
                
            }
        }
        foreach (var item in chunksList)
        {
            GenerateMeshes(item, LODLevels.LOD2);
            yield return new WaitForSeconds(0.05f);
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
        }
        
    }
    void GenerateMeshes(chunkObject chunk,LODLevels quality)
    {
        float resolution = 0;
        switch (quality)
        {
            case LODLevels.LOD0:
                resolution = 129;
                break;
            case LODLevels.LOD1:
                resolution = 65;
                break;
            case LODLevels.LOD2:
                resolution = 17;
                break;
            case LODLevels.LOD3:
                resolution = 9;
                break;
        }
        Mesh tempMesh = new Mesh();
        List<Vector3> tempVerts = new List<Vector3>();
        List<int> tris = new List<int>();


        float[,] lastPixel = new float[noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution];
        float[,] noiseMap = new float[(int)resolution + 1, (int)resolution + 1];
        for (int i = 0; i < noiseMapGenerator.noiseLayers.Count; i++)
        {
            LayerGen noiseGen = noiseMapGenerator.noiseLayers[i];
            for (int x = 0; x < resolution+1; x++)
            {
                for (int z = 0; z < resolution+1; z++)
                {
                    float posx = ((x / resolution) * width) + (chunk.position.x);
                    float posz = ((z / resolution) * width) + (chunk.position.z);
                    float noiseFloat = noiseGen.getNoiseMap(posx, posz);
                    
                        switch (noiseGen.blendMode)
                        {
                            case LayerGen.BlendModes.Multiply:
                                noiseFloat = noiseFloat * lastPixel[x, z];

                                break;
                            case LayerGen.BlendModes.Subtract:
                                noiseFloat =  lastPixel[x, z] - noiseFloat;

                                break;
                            case LayerGen.BlendModes.Add:
                                noiseFloat = noiseFloat + lastPixel[x, z];

                                break;
                            case LayerGen.BlendModes.Divide:
                                noiseFloat =  lastPixel[x, z]/noiseFloat;
                                break;
                            case LayerGen.BlendModes.Mask:
                                if (noiseFloat < lastPixel[x, z] + noiseGen.min)
                                {
                                    noiseFloat = lastPixel[x, z];
                                }
                                if(noiseFloat > lastPixel[x, z] + noiseGen.max)
                                {
                                    noiseFloat = lastPixel[x, z];
                                }
                                break;
                            default:
                                break;
                        }
                    noiseMap[x, z] = noiseFloat;
                    lastPixel[x, z] = noiseFloat;
                   
                    
                }
            }

            
            
        }
        for (int x = 0; x < resolution+1; x++)
        {
            for (int z = 0; z < resolution+1; z++)
            {

                tempVerts.Add(new Vector3(((x / resolution) * width) - (width / 2f), noiseMap[x, z],((z / resolution) * width) - (width / 2)));
            }
        }
       
        int trisIndex = 0;
        int vert = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int i = 0; i < resolution; i++)
            {
                tris.Add(vert);
                tris.Add((vert + 1));
                tris.Add((int)(vert + resolution + 1 + 1));
                tris.Add(vert);
                tris.Add((int)(vert + resolution + 1 + 1));
                tris.Add((int)(vert + resolution + 1));
                vert++;
                trisIndex += 6;
            }
            vert++;
        }
        
        tempMesh.bounds = new Bounds(chunk.position, new Vector3(width, height, width));
        tempMesh.vertices = tempVerts.ToArray();
        tempMesh.triangles = tris.ToArray();
        chunk.setMesh(tempMesh);
        chunk.verts = tempVerts.ToArray();
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


        public chunkObject(GameObject chunk,Vector3Int position,Transform parent,Material tempMaterial)
        {
            chunkGameObject = chunk;
            Vector3Int newPos = new Vector3Int(position.x, 0, position.z);
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
