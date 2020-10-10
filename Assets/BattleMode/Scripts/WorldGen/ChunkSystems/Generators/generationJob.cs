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

        [BurstCompile]
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
            [ReadOnly]
            public int numberOfSettings;

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

            private float min;
            private float max;

            public float minHeight;
            public float maxHeight;

            public void Execute()
            {

                for (int j = 0; j < layerSettings.Length / numberOfSettings; j++)
                {
                    octaves = (int)layerSettings[(j * numberOfSettings)];
                    multiplier = (int)layerSettings[(j * numberOfSettings) + 1];
                    amplitute = layerSettings[(j * numberOfSettings) + 2];
                    lacunarity = layerSettings[(j * numberOfSettings) + 3];
                    persistance = layerSettings[(j * numberOfSettings) + 4];
                    sizeScalex = layerSettings[(j * numberOfSettings) + 5];
                    sizeScalez = layerSettings[(j * numberOfSettings) + 6];
                    blendMode = (BlendModes)layerSettings[(j * numberOfSettings) + 7];
                    MultiplicationOfFinal = layerSettings[(j * numberOfSettings) + 8];
                    subtraction = layerSettings[(j * numberOfSettings) + 9];
                    offsetX = layerSettings[(j * numberOfSettings) + 10];
                    offsetY = layerSettings[(j * numberOfSettings) + 11];
                    offsetZ = layerSettings[(j * numberOfSettings) + 12];

                    min = layerSettings[(j * numberOfSettings) + 13];
                    max = layerSettings[(j * numberOfSettings) + 14];

                    /*Debug.Log("Properties from Layer" + j + ": " +
                        "\n + octaves :" + octaves +
                        "\n + multiplier :" + multiplier +
                        "\n + amplitute :" + amplitute +
                        "\n + lacunarity :" + lacunarity +
                        "\n + persistance :" + persistance +
                        "\n + sizeScalex :" + sizeScalex +
                        "\n + sizeScalez :" + sizeScalez +
                        "\n + blendMode :" + blendMode +
                        "\n + MultiplicationOfFinal :" + MultiplicationOfFinal +
                        "\n + subtraction :" + subtraction +
                        "\n + min :" + min +
                        "\n + max :" + max
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
                            float noiseFloat = coherentNoise((xCord + offsetX) * sizeScalex, offsetY, (yCord + offsetZ) * sizeScalez, octaves, multiplier, amplitute, lacunarity, persistance);
                            noiseFloat = (noiseFloat + 1) / 2;
                            
                            noiseFloat *= MultiplicationOfFinal;
                            noiseFloat -= subtraction;
                            

                            if (noiseFloat < min && blendMode != BlendModes.Mask)
                            {
                                noiseFloat = min;
                                blendMode = BlendModes.Add;
                            }
                            if (noiseFloat > max && blendMode != BlendModes.Mask)
                            {
                                noiseFloat = max;
                                blendMode = BlendModes.Add;
                            }
                            


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
                                    if (lastnoiseLayeredMap[i] < max && lastnoiseLayeredMap[i] > min)
                                    {
                                        noiseFloat += lastnoiseLayeredMap[i];
                                    }
                                    else
                                    {
                                        noiseFloat = lastnoiseLayeredMap[i];
                                    }
                                    break;
                                default:
                                    break;
                            }
                            blendMode = (BlendModes)layerSettings[(j * numberOfSettings) + 7];
                            noiseLayeredMap[i] = noiseFloat;
                            lastnoiseLayeredMap[i] = noiseFloat;

                            //noiseLayeredMap[i] = coherentNoise(xCord, 0, yCord, octaves, multiplier, amplitute, lacunarity, persistance) * MultiplicationOfFinal;
                            i++;
                        }
                    }
                }
                /*
                foreach (var item in noiseLayeredMap)
                {
                    if (item < minHeight)
                        minHeight = item;
                    if (item > maxHeight)
                        maxHeight = item;
                }

                */
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
        [BurstCompile]
        public struct ChunkGenerate : IJob
        {
            public NativeArray<Vector3> verts;
            public NativeArray<Vector3> normals;

            public NativeArray<Vector2> uvs;

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
                        uvs[i] = new Vector2(x, z);
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
                uvs.Dispose();
            }
        }
    }
}
