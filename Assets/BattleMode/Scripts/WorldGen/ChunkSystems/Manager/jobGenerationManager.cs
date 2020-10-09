using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public int numberOfSettings = 16;


    public int jobAmount = 0;
    public int meshJobAmount = 0;

    public void initalizeManager(WorldTypes type)
    {
        typeOfWorld = type;
        StartCoroutine(checkJobList());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 50, 100), new GUIContent("FillJobs" + jobAmount.ToString()+"\n MeshJobs :"+ meshJobAmount));
    }
    public IEnumerator checkJobList()
    {

        List<JobHandle> jobsToRemove = new List<JobHandle>();
        while (cleanJobList)
        {
            
            foreach (var item in jobFillList)
            {
                if (item.Key.IsCompleted)
                {
                    item.Key.Complete();
                    //item.Value._pos.displayChunk();
                    GenerateMesh(item.Value._pos, item.Value.noiseLayeredMap, item.Value.chunkRes, item.Value.LOD);
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
                        Chunk c = new Chunk(item.Value.cp)
                        {
                            Verticies = item.Value.verts.ToList(),
                            Indices = item.Value.tris.ToList(),
                            norm = item.Value.normals.ToList(),
                            uv = item.Value.uvs.ToList()
                        };
                        c.GenerateObject(defaultMaterial, new Vector3((c.Position.X * chunkSize) - ((typeOfWorld.sizeX * chunkSize) / 2f), 0, (c.Position.Z * chunkSize) - ((typeOfWorld.sizeZ * chunkSize) / 2f)), transform, item.Value.resolution, item.Value.lod);
                        _chunks.Add(item.Value.cp, c);
                    }
                    else
                    {
                        Chunk c = _chunks[item.Value.cp];
                        c.Verticies = item.Value.verts.ToList();
                        c.Indices = item.Value.tris.ToList();
                        c.norm = item.Value.normals.ToList();
                        c.uv = item.Value.uvs.ToList();
                        c.updateMesh(item.Value.lod);
                    }

                    item.Value.Dispose();
                    jobsToRemove.Add(item.Key);
                }
            }
            foreach (JobHandle item in jobsToRemove.ToArray())
            {
                if (jobFillList.ContainsKey(item))
                {
                    jobFillList.Remove(item);
                    jobsToRemove.Remove(item);
                }

                if (jobMeshList.ContainsKey(item))
                {
                    jobMeshList.Remove(item);
                    jobsToRemove.Remove(item);
                }
            }

            yield return new WaitForSeconds(0.125f);
        }
    }

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

    public void GenerateChunkAt(ChunkPoint cp, LODLEVELS lod)
    {
        jobAmount = jobFillList.Count;
        meshJobAmount = jobMeshList.Count;
        if (_chunks.ContainsKey(cp) && _chunks[cp].hasMesh(lod))
            _chunks[cp].updateMesh(lod);
        else
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
                    chunkRes = 48;
                    break;
                case LODLEVELS.LOD3:
                    chunkRes = 32;
                    break;
                case LODLEVELS.LOD4:
                    chunkRes = 1;
                    break;
                default:
                    chunkRes = 2;
                    break;
            }
            NativeArray<float> naArray = prepareLayerSettings(typeOfWorld);

            NativeArray<float> noiseMapArr = new NativeArray<float>((chunkRes + extraRows) * (chunkRes + extraRows), Allocator.Persistent);
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
                chunkSize = chunkSize,
                numberOfSettings = numberOfSettings

            };
            JobHandle jh = job.Schedule();
            jobFillList.Add(jh, job);
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
        NativeArray<float> naArray = new NativeArray<float>(layerSettings.Length, Allocator.Persistent);
        naArray.CopyFrom(layerSettings);

        return naArray;
    }


}
