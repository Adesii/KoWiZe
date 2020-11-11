using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Tilemaps;
using WorldGenJobs;
using static World;

public class jobGenerationManager : MonoBehaviour
{

    public struct JobHolder
    {
        public generationJob.fillNoiseMap Value;
        public JobHandle Key;
    }

    List<JobHolder> jobFillList = new List<JobHolder>();
    //Dictionary<JobHandle, generationJob.ChunkGenerate> jobMeshList = new Dictionary<JobHandle, generationJob.ChunkGenerate>();
    private WorldTypes typeOfWorld;

    public bool cleanJobList = true;
    public Material defaultMaterial;
    public int extraRows = 1;
    public int numberOfSettings = 16;


    public int jobAmount = 0;
    public int meshJobAmount = 0;
    List<JobHolder> jobsToRemove = new List<JobHolder>();
    public void initalizeManager(WorldTypes type)
    {
        typeOfWorld = type;
        //StartCoroutine(checkJobList());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 50, 100), new GUIContent("FillJobs" + jobAmount.ToString() + "\n MeshJobs :" + meshJobAmount));
    }

    bool BreakingUP = true;
    private void LateUpdate()
    {
        int iterations = 0;
        foreach (var item in jobFillList)
        {
            if (item.Key.IsCompleted)
            {
                item.Key.Complete();
                if (!_chunks.ContainsKey(item.Value._pos))
                {
                    Chunk c = new Chunk(item.Value._pos)
                    {
                        Verticies = item.Value.verts.ToArray(),
                        Indices = item.Value.tris.ToArray(),
                        norm = item.Value.normals.ToArray(),
                        uv = item.Value.uvs.ToArray()
                    };
                    c.GenerateObject(defaultMaterial, new Vector3((c.Position.X * chunkSize) - ((typeOfWorld.sizeX * chunkSize) / 2f), 0, (c.Position.Z * chunkSize) - ((typeOfWorld.sizeZ * chunkSize) / 2f)), transform, item.Value.chunkRes, item.Value.LOD);
                    _chunks.Add(item.Value._pos, c);
                }
                else
                {
                    Chunk c = _chunks[item.Value._pos];
                    c.Verticies = item.Value.verts.ToArray();
                    c.Indices = item.Value.tris.ToArray();
                    c.norm = item.Value.normals.ToArray();
                    c.uv = item.Value.uvs.ToArray();
                    c.updateMesh(item.Value.LOD);
                }

                item.Value.Dispose();
                jobsToRemove.Add(item);
            }
            iterations++;/*
            if (iterations > jobFillList.Count/6)
            {
                break;
            }
            */
        }
        foreach (JobHolder item in jobsToRemove)
        {
            jobFillList.Remove(item);
        }
    }

    /*
    public IEnumerator checkJobList()
    {

        List<JobHolder> jobsToRemove = new List<JobHolder>();
        while (cleanJobList)
        {
            jobsToRemove.Clear();

            for (int i = 0; i < jobFillList.Count; i++)
            {
                JobHolder item = jobFillList[i];
                if (item.Key.IsCompleted)
                {
                    item.Key.Complete();
                    if (!_chunks.ContainsKey(item.Value._pos))
                    {
                        Chunk c = new Chunk(item.Value._pos)
                        {
                            Verticies = item.Value.verts.ToList(),
                            Indices = item.Value.tris.ToList(),
                            norm = item.Value.normals.ToList(),
                            uv = item.Value.uvs.ToList()
                        };
                        c.GenerateObject(defaultMaterial, new Vector3((c.Position.X * chunkSize) - ((typeOfWorld.sizeX * chunkSize) / 2f), 0, (c.Position.Z * chunkSize) - ((typeOfWorld.sizeZ * chunkSize) / 2f)), transform, item.Value.chunkRes, item.Value.LOD);
                        _chunks.Add(item.Value._pos, c);
                    }
                    else
                    {
                        Chunk c = _chunks[item.Value._pos];
                        c.Verticies = item.Value.verts.ToList();
                        c.Indices = item.Value.tris.ToList();
                        c.norm = item.Value.normals.ToList();
                        c.uv = item.Value.uvs.ToList();
                        c.updateMesh(item.Value.LOD);
                    }

                    item.Value.Dispose();
                    jobsToRemove.Add(item);
                }
            }
            foreach (JobHolder item in jobsToRemove)
            {
                jobFillList.Remove(item);
            }
            yield return new WaitForSeconds(0.125f);
        }
    }
    */
    /*
    private void GenerateMesh(ChunkPoint cp, NativeArray<float> noiseMap, int resolution, LODLEVELS lod)
    {


        NativeArray<Vector3> verts = new NativeArray<Vector3>((resolution + extraRows) * (resolution + extraRows), Allocator.Persistent);
        NativeArray<int> tris = new NativeArray<int>(verts.Length * 6, Allocator.Persistent);
        NativeArray<Vector3> normals = new NativeArray<Vector3>((resolution + extraRows) * (resolution + extraRows), Allocator.Persistent);
        NativeArray<Vector2> uvs = new NativeArray<Vector2>((resolution + extraRows) * (resolution + extraRows), Allocator.Persistent);

        generationJob.ChunkGenerate job = new generationJob.ChunkGenerate
        {
            noiseLayeredMap = noiseMap,
            verts = verts,
            cp = cp,
            tris = tris,
            resolution = resolution,
            normals = normals,
            lod = lod,
            chunkSize = chunkSize,
            uvs = uvs
        };
        JobHandle jh = job.Schedule();
        jobMeshList.Add(jh, job);
    }
    */
    public void GenerateChunkAt(ChunkPoint cp, LODLEVELS lod)
    {
        jobAmount = jobFillList.Count;
        //meshJobAmount = jobMeshList.Count;



        if (_chunks.ContainsKey(cp) && _chunks[cp].hasMesh(lod))
            _chunks[cp].updateMesh(lod);
        else
        {
            if (lod < LODLEVELS.LOD1 || lod > LODLEVELS.LOD2)
            {
                int chunkRes;
                switch (lod)
                {
                    case LODLEVELS.LOD0:
                        chunkRes = 200;
                        break;
                    case LODLEVELS.LOD1:
                        chunkRes = 128;
                        break;
                    case LODLEVELS.LOD2:
                        chunkRes = 32;
                        break;
                    case LODLEVELS.LOD3:
                        chunkRes = 4;
                        break;
                    case LODLEVELS.LOD4:
                        chunkRes = 4;
                        break;
                    default:
                        chunkRes = 2;
                        break;
                }

                //if (jobFillList.Count < 30)
                //{
                NativeArray<float> naArray = prepareLayerSettings(typeOfWorld);
                NativeArray<float> noiseMapArr = new NativeArray<float>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.TempJob);
                NativeArray<float> lastnoiseMapArr = new NativeArray<float>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.TempJob);
                NativeArray<Vector3> verts = new NativeArray<Vector3>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.TempJob);
                NativeArray<int> tris = new NativeArray<int>(verts.Length * 6, Allocator.TempJob);
                NativeArray<Vector3> normals = new NativeArray<Vector3>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.TempJob);
                NativeArray<Vector2> uvs = new NativeArray<Vector2>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.TempJob);




                generationJob.fillNoiseMap job = new generationJob.fillNoiseMap
                {
                    noiseLayeredMap = noiseMapArr,
                    lastnoiseLayeredMap = lastnoiseMapArr,
                    _pos = cp,
                    layerSettings = naArray,
                    numberOfLayers = typeOfWorld.noiseLayers.Count,
                    chunkRes = chunkRes,
                    LOD = lod,
                    chunkSize = chunkSize,
                    numberOfSettings = numberOfSettings,

                    verts = verts,
                    uvs = uvs,
                    normals = normals,
                    tris = tris

                };



                JobHandle jh = job.Schedule();
                JobHolder jhh = new JobHolder { Key = jh, Value = job };
                jobFillList.Add(jhh);
                // }
                //else
                //{
                //}
            }
        }
    }
    private NativeArray<float> prepareLayerSettings(WorldTypes worldType)
    {

        float[] layerSettings = new float[worldType.noiseLayers.Count * numberOfSettings];
        for (int i = 0; i < worldType.noiseLayers.Count; i++)
        {
            LayerGen currWorld = worldType.noiseLayers[i];
            layerSettings[i * numberOfSettings] = currWorld.octaves;
            layerSettings[(i * numberOfSettings) + 1] = currWorld.multiplier;
            layerSettings[(i * numberOfSettings) + 2] = currWorld.amplitute;
            layerSettings[(i * numberOfSettings) + 3] = currWorld.lacunarity;
            layerSettings[(i * numberOfSettings) + 4] = currWorld.persistance;
            layerSettings[(i * numberOfSettings) + 5] = currWorld.sizeScalex;
            layerSettings[(i * numberOfSettings) + 6] = currWorld.sizeScalez;
            layerSettings[(i * numberOfSettings) + 7] = (float)currWorld.blendMode;
            layerSettings[(i * numberOfSettings) + 8] = currWorld.MultiplicationOfFinal;
            layerSettings[(i * numberOfSettings) + 9] = currWorld.subtraction;
            layerSettings[(i * numberOfSettings) + 10] = currWorld.offset.x;
            layerSettings[(i * numberOfSettings) + 11] = currWorld.offset.y;
            layerSettings[(i * numberOfSettings) + 12] = currWorld.offset.z;

            layerSettings[(i * numberOfSettings) + 13] = currWorld.min;
            layerSettings[(i * numberOfSettings) + 14] = currWorld.max;


        }
        NativeArray<float> naArray = new NativeArray<float>(layerSettings.Length, Allocator.TempJob);
        naArray.CopyFrom(layerSettings);

        return naArray;
    }


}
