using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using WorldGenJobs;
using static World;

public class jobGenerationManager : MonoBehaviour
{
    Dictionary<JobHandle, generationJob.fillNoiseMap> jobFillList = new Dictionary<JobHandle, generationJob.fillNoiseMap>();
    Dictionary<JobHandle, generationJob.ChunkGenerate> jobMeshList = new Dictionary<JobHandle, generationJob.ChunkGenerate>();
    private WorldTypes typeOfWorld;

    public bool cleanJobList = true;
    public Material defaultMaterial;
    public int extraRows = 1;


    public void initalizeManager(WorldTypes type)
    {
        typeOfWorld = type;
        StartCoroutine(checkJobList());
    }


    public IEnumerator checkJobList()
    {
        List<JobHandle> jobsToRemove = new List<JobHandle>();
        while (cleanJobList)
        {
            jobsToRemove.Clear();
            foreach (var item in jobFillList)
            {

                if (item.Key.IsCompleted)
                {

                    item.Key.Complete();
                    //item.Value._pos.displayChunk();
                    GenerateMesh(item.Value._pos, item.Value.noiseLayeredMap,item.Value.chunkRes,item.Value.LOD);
                    item.Value.Dispose();
                    jobsToRemove.Add(item.Key);
                }
            }
            foreach (var item in jobMeshList)
            {

                if (item.Key.IsCompleted)
                {
                    item.Key.Complete();
                    if (!_chunks.ContainsKey(item.Value.cp))
                    {
                        Chunk c = new Chunk(item.Value.cp);
                        c.Verticies = item.Value.verts.ToList();
                        c.Indices = item.Value.tris.ToList();
                        c.norm = item.Value.normals.ToList();
                        c.GenerateObject(defaultMaterial, new Vector3((c.Position.X * chunkSize) - ((typeOfWorld.sizeX * chunkSize) / 2f), 0, (c.Position.Z * chunkSize) - ((typeOfWorld.sizeZ * chunkSize) / 2f)), transform, item.Value.resolution, item.Value.lod);
                        _chunks.Add(item.Value.cp, c);
                    }
                    else
                    {
                        Chunk c = _chunks[item.Value.cp];
                        c.Verticies = item.Value.verts.ToList();
                        c.Indices = item.Value.tris.ToList();
                        c.norm = item.Value.normals.ToList();
                        c.updateMesh(item.Value.lod);
                    }
                    item.Value.Dispose();
                    jobsToRemove.Add(item.Key);
                }
            }
            foreach (JobHandle item in jobsToRemove)
            {
                if (jobFillList.ContainsKey(item))
                    jobFillList.Remove(item);
                if (jobMeshList.ContainsKey(item))
                    jobMeshList.Remove(item);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void GenerateMesh(ChunkPoint cp, NativeArray<float> noiseMap,int resolution,LODLEVELS lod)
    {


        NativeArray<Vector3> verts = new NativeArray<Vector3>((resolution+ extraRows) * (resolution+ extraRows), Allocator.Persistent);
        NativeArray<int> tris = new NativeArray<int>(verts.Length* 6, Allocator.Persistent);
        NativeArray<Vector3> normals = new NativeArray<Vector3>((resolution + extraRows) * (resolution + extraRows), Allocator.Persistent);

        generationJob.ChunkGenerate job = new generationJob.ChunkGenerate
        {
            noiseLayeredMap = noiseMap,
            verts = verts,
            cp = cp,
            tris = tris,
            resolution = resolution,
            normals = normals,
            lod = lod,
            chunkSize = chunkSize
        };
        JobHandle jh = job.Schedule();
        jobMeshList.Add(jh, job);
    }

    public void GenerateChunkAt(ChunkPoint cp, LODLEVELS lod)
    {

        int chunkRes = 0;
        switch (lod)
        {
            case LODLEVELS.LOD0:
                chunkRes = 128;
                break;
            case LODLEVELS.LOD1:
                chunkRes = 64;
                break;
            case LODLEVELS.LOD2:
                chunkRes = 48;
                break;
            case LODLEVELS.LOD3:
                chunkRes = 3;
                break;
            default: 
                chunkRes = 2;
                break;
        }
        NativeArray<float> naArray = prepareLayerSettings(typeOfWorld);
        NativeArray<float> noiseMapArr = new NativeArray<float>((chunkRes+ extraRows) * (chunkRes+ extraRows), Allocator.Persistent);
        NativeArray<float> lastnoiseMapArr = new NativeArray<float>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.Persistent);

        generationJob.fillNoiseMap job = new generationJob.fillNoiseMap
        {
            noiseLayeredMap = noiseMapArr,
            lastnoiseLayeredMap = lastnoiseMapArr,
            _pos = cp,
            layerSettings = naArray,
            numberOfLayers = typeOfWorld.noiseLayers.Count,
            chunkRes = chunkRes,
            LOD = lod,
            chunkSize = chunkSize
        };
        JobHandle jh = job.Schedule();
        jobFillList.Add(jh, job);

    }
    private NativeArray<float> prepareLayerSettings(WorldTypes worldType)
    {
        float[] layerSettings = new float[worldType.noiseLayers.Count * 13];
        for (int i = 0; i < worldType.noiseLayers.Count; i++)
        {
            LayerGen currWorld = worldType.noiseLayers[i];
            layerSettings[i * 13] = currWorld.octaves;
            layerSettings[(i * 13) + 1] = currWorld.multiplier;
            layerSettings[(i * 13) + 2] = currWorld.amplitute;
            layerSettings[(i * 13) + 3] = currWorld.lacunarity;
            layerSettings[(i * 13) + 4] = currWorld.persistance;
            layerSettings[(i * 13) + 5] = currWorld.sizeScalex;
            layerSettings[(i * 13) + 6] = currWorld.sizeScalez;
            layerSettings[(i * 13) + 7] = (float)currWorld.blendMode;
            layerSettings[(i * 13) + 8] = currWorld.MultiplicationOfFinal;
            layerSettings[(i * 13) + 9] = currWorld.subtraction;
            layerSettings[(i * 13) + 10] = currWorld.offset.x;
            layerSettings[(i * 13) + 11] = currWorld.offset.y;
            layerSettings[(i * 13) + 12] = currWorld.offset.z;


        }
        NativeArray<float> naArray = new NativeArray<float>(layerSettings.Length, Allocator.Persistent);
        naArray.CopyFrom(layerSettings);

        return naArray;
    }


}
