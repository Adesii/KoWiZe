using System.Collections;
using Unity.Burst;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static LayerGen;
namespace WorldGenJobs
{



    public class generationJob : MonoBehaviour
    {


        [BurstCompile(CompileSynchronously = true)]
        public struct fillNoiseMap : IJob
        {
            public NativeArray<float> noiseLayeredMap;
            public NativeArray<float> lastnoiseLayeredMap;

            public World.ChunkPoint _pos;
            public int numberOfLayers;
            public NativeArray<float> layerSettings;
            public int chunkRes;
            [ReadOnly]
            public World.LODLEVELS LOD;
            [ReadOnly]
            public int chunkSize;

            private int layerIndex;

            private int octaves;
            private int multiplier;
            private float amplitute;
            private float lacunarity;
            private float persistance;

            private float sizeScalex;
            private float sizeScalez;
            private BlendModes blendMode;

            private float MultiplicationOfFinal;
            private float subtraction;

            private float offsetX;
            private float offsetY;
            private float offsetZ;

            public void Execute()
            {
                for (int j = 0; j < layerSettings.Length / 13; j++)
                {
                    octaves = (int)layerSettings[(j * 13)];
                    multiplier = (int)layerSettings[(j * 13) + 1];
                    amplitute = layerSettings[(j * 13) + 2];
                    lacunarity = layerSettings[(j * 13) + 3];
                    persistance = layerSettings[(j * 13) + 4];
                    sizeScalex = layerSettings[(j * 13) + 5];
                    sizeScalez = layerSettings[(j * 13) + 6];
                    blendMode = (BlendModes)layerSettings[(j * 13) + 7];
                    MultiplicationOfFinal = layerSettings[(j * 13) + 8];
                    subtraction = layerSettings[(j * 13) + 9];
                    offsetX = layerSettings[(j * 13) + 10];
                    offsetY = layerSettings[(j * 13) + 11];
                    offsetZ = layerSettings[(j * 13) + 12];




                    /*Debug.Log("Properties from Layer"+j+": " +
                        "\n "+octaves+
                        "\n " + multiplier +
                        "\n " + amplitute +
                        "\n " + lacunarity +
                        "\n " + persistance +
                        "\n " + sizeScalex +
                        "\n " + sizeScalez +
                        "\n " + blendMode +
                        "\n " + MultiplicationOfFinal +
                        "\n " + subtraction
                        );
                    */

                    int i = 0;
                    for (int x = 0; x <= chunkRes; x++)
                    {
                        for (int z = 0; z <= chunkRes; z++)
                        {
                            //float xCord = ((((float)x) / (float)chunkRes) * (float)chunkSize) + _pos.X;
                            float xCord = (((float)x / (float)chunkRes) * (float)chunkSize) + ((float)_pos.X * chunkSize);
                            //float yCord = ((((float)z) / (float)chunkRes) * (float)chunkSize) + _pos.Z;
                            float yCord = (((float)z / (float)chunkRes) * (float)chunkSize) + ((float)_pos.Z * chunkSize);
                            float noiseFloat = coherentNoise(xCord + offsetX, offsetY, yCord + offsetZ, octaves, multiplier, amplitute, lacunarity, persistance);
                            noiseFloat = (noiseFloat + 1) / 2;
                            noiseFloat *= MultiplicationOfFinal;
                            noiseFloat -= subtraction;

                            switch (blendMode)
                            {
                                case LayerGen.BlendModes.Multiply:
                                    noiseFloat *= lastnoiseLayeredMap[i];

                                    break;
                                case LayerGen.BlendModes.Subtract:
                                    noiseFloat = lastnoiseLayeredMap[i] - noiseFloat;

                                    break;
                                case LayerGen.BlendModes.Add:
                                    noiseFloat += lastnoiseLayeredMap[i];

                                    break;
                                case LayerGen.BlendModes.Divide:
                                    noiseFloat = lastnoiseLayeredMap[i] / noiseFloat;
                                    break;
                                case LayerGen.BlendModes.Mask:
                                    if (noiseFloat < lastnoiseLayeredMap[i])
                                    {
                                        noiseFloat = noiseLayeredMap[i];

                                    }
                                    else if(noiseFloat < lastnoiseLayeredMap[i])
                                    {
                                        noiseFloat += noiseLayeredMap[i];
                                    }
                                    break;
                                default:
                                    break;
                            }

                            noiseLayeredMap[i] = noiseFloat;
                            lastnoiseLayeredMap[i] = noiseFloat;
                            //noiseLayeredMap[i] = coherentNoise(xCord, 0, yCord, octaves, multiplier, amplitute, lacunarity, persistance) * MultiplicationOfFinal;
                            i++;
                        }
                    }

                }



            }
            public void Dispose()
            {
                layerSettings.Dispose();
                lastnoiseLayeredMap.Dispose();
            }

            public float coherentNoise(float x, float y, float z, int octaves = 1, int multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f)
            {
                Vector3 v3 = new Vector3(x, y, z) / multiplier;
                float val = 0;
                for (int n = 0; n < octaves; n++)
                {
                    val += noise.snoise(new float3(v3.x, v3.y, v3.z)) * amplitude;
                    v3 *= lacunarity;
                    amplitude *= persistence;
                }
                return val;
            }


        }
        [BurstCompile(CompileSynchronously = true)]
        public struct ChunkGenerate : IJob
        {
            public NativeArray<Vector3> verts;
            public NativeArray<Vector3> normals;
            public NativeArray<int> tris;
            public NativeArray<float> noiseLayeredMap;
            public World.ChunkPoint cp;
            public int resolution;
            [ReadOnly]
            public World.LODLEVELS lod;
            [ReadOnly]
            public int chunkSize;

            public void Execute()
            {
                int i = 0;
                for (int x = 0; x <= resolution; x++)
                {
                    for (int z = 0; z <= resolution; z++)
                    {
                        verts[i] = new Vector3(((float)x / (float)resolution) * (float)chunkSize, noiseLayeredMap[i], ((float)z / (float)resolution) * (float)chunkSize);
                        normals[i] = new Vector3(0, 1, 0);
                        i++;
                    }
                }
                for (int ti = 0, vi = 0, y = 0; y < resolution; y++, vi++)
                {
                    for (int x = 0; x < resolution; x++, ti += 6, vi++)
                    {
                        tris[ti + 5] = vi + resolution + 2;
                        tris[ti + 4] = tris[ti + 1] = vi + 1;
                        tris[ti + 3] = tris[ti + 2] = vi + resolution + 1;
                        tris[ti] = vi;

                    }
                }
            }

            public void Dispose()
            {
                noiseLayeredMap.Dispose();
                verts.Dispose();
                tris.Dispose();
                normals.Dispose();
            }
        }
    }
}
