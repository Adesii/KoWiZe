using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunkGenerator : MonoBehaviour
{
    //ChunkSettings
    public static int height = 32;
    public static int width = 32;
    public bool LiveEdit = false;
    public int LODRadius = 3;

    public GameObject player;


    public Material tempMaterial;

    public WorldTypes noiseMapGenerator;
    float[,] noiseMap;

    public LODLevels defaultLOD;

    public enum LODLevels
    {
        LOD0,
        LOD1,
        LOD2,
        LOD3
    }

    //world Settings

    //generic Variables
    chunkObject[,] chunksList;
    List<chunkObject> activeChunks = new List<chunkObject>();


    chunkObject debugDrawChunk;

    // Start is called before the first frame update
    void Start()
    {
        chunksList = new chunkObject[noiseMapGenerator.sizeX,noiseMapGenerator.sizeZ];
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
                GenerateMeshes(item, defaultLOD);
        }
        }
        updateLOD();
    }

    public void updateLOD()
    {
        for (int x = -LODRadius; x < LODRadius; x++)
        {
            for (int z = -LODRadius; z < LODRadius; z++)
            {
                chunkObject chunk=getchunkFromPosition(new Vector3(player.transform.position.x+(x*width),0,player.transform.position.z+(z*height)));
                if (!chunk.updateLOD((LODLevels)(Mathf.Clamp(Math.Abs(x), 0, 4))))
                {
                    updateChunk(chunk, (LODLevels)(Mathf.Clamp(Math.Abs(x), 0, 4)));
                }
                if(!activeChunks.Contains(chunk)) activeChunks.Add(chunk);
                
            }
        }
        //Debug.Log(activeChunks.Count);
        foreach (chunkObject item in activeChunks)
        {
            if(item.currentLOD == LODLevels.LOD3)
            {
                activeChunks.Remove(item);
            }
        }
    }

    public void updateChunk(chunkObject chunk, LODLevels lod)
    {
        GenerateMeshes(chunk, lod);
    }
    public chunkObject getchunkFromPosition(Vector3 position)
    {
        //Debug.Log(position);
        //Debug.Log(chunksList.Length);
        float posX = position.x + ((noiseMapGenerator.sizeX * width) / 2);
        float posZ = position.z + ((noiseMapGenerator.sizeZ * height) / 2);

        //Debug.DrawLine(new Vector3(posX, -100, posZ), new Vector3(posX, 100, posZ));
        //Debug.Log(posX);
        //Debug.Log(Mathf.FloorToInt(posX/ width));
        //Debug.Log(Mathf.FloorToInt(posZ/height));
        
        return chunksList[Mathf.FloorToInt(posX / width), Mathf.FloorToInt(posZ / height)];
    }
    public void RegenerateALL()
    {
        StopAllCoroutines();
        chunksList = new chunkObject[noiseMapGenerator.sizeX, noiseMapGenerator.sizeZ];
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
                chunkObject chunkGameObject = new chunkObject(new GameObject("Chunk_" + x + "_" + z), new Vector3Int((int)(x * width), 0, (int)(z * width)), transform, tempMaterial,new Vector2(noiseMapGenerator.sizeX,noiseMapGenerator.sizeZ));
                chunksList[x,z] = chunkGameObject;
                
                
            }
        }
        foreach (var item in chunksList)
        {
                GenerateMeshes(item, defaultLOD);
        }
        yield return new WaitForEndOfFrame();
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
                resolution = 65;
                break;
            case LODLevels.LOD1:
                resolution = 33;
                break;
            case LODLevels.LOD2:
                resolution = 9;
                break;
            case LODLevels.LOD3:
                resolution = 3;
                break;
        }
        Mesh tempMesh = new Mesh();
        List<Vector3> tempVerts = new List<Vector3>();
        List<int> tris = new List<int>();


        float[,] lastPixel = new float[(int)resolution + 1, (int)resolution + 1];
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
                    float noiseFloat = 0;
                    
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
                                if (noiseFloat > lastPixel[x, z])
                                {
                                    noiseFloat = noiseMap[x, z];
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

        //smoothIteration(noiseMap);
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
        chunk.setMesh(tempMesh,quality);
        chunk.verts = tempVerts.ToArray();
        debugDrawChunk = chunk;
        
    }
    /*
    private void smoothIteration(float[,] noiseMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                noiseMap[x,z]
            }
        }
    }
    */
    public class chunkObject
    {
        public GameObject chunkGameObject;
        public Vector3Int position;
        public MeshFilter meshFil;
        public MeshRenderer meshRen;
        public MeshCollider meshCol;
        public Mesh mesh;
        public Vector3[] verts; 

        public LODLevels currentLOD;
        public Dictionary<LODLevels,Mesh> savedMeshes = new Dictionary<LODLevels, Mesh>();



        public chunkObject(GameObject chunk,Vector3Int position,Transform parent,Material tempMaterial,Vector2 Mapsize)
        {
            chunkGameObject = chunk;
            Vector3Int newPos = new Vector3Int(position.x, 0, position.z);
            this.position = newPos;
            meshFil=chunkGameObject.AddComponent<MeshFilter>();
            meshRen = chunkGameObject.AddComponent<MeshRenderer>();
            meshCol = chunkGameObject.AddComponent<MeshCollider>();
            chunkGameObject.transform.parent = parent;
            chunkGameObject.transform.position = new Vector3(position.x-((Mapsize.x*width)/2),position.y,position.z-((Mapsize.y*height)/2));
            meshRen.material = tempMaterial;
        }
        public void setMesh(Mesh newMesh,LODLevels meshLOD)
        {
            if(!savedMeshes.ContainsKey(meshLOD))
            {
                savedMeshes.Add(meshLOD, newMesh);

                newMesh.RecalculateNormals();
                if (meshCol.sharedMesh == null)
                {
                    meshCol.sharedMesh = newMesh;
                }
                mesh = newMesh;
                meshFil.mesh = newMesh;
            }
            else
            {
                mesh = savedMeshes[meshLOD];
                meshFil.mesh = savedMeshes[meshLOD];
            }

        }
        public bool updateLOD(LODLevels lod)
        {
            if (savedMeshes.ContainsKey(lod))
            {
                mesh = savedMeshes[lod];
                meshFil.mesh = savedMeshes[lod];
                return true;
            }
            else
            {
                return false;
            }
        }
    }




}
