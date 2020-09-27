using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "WorldType",menuName ="KoWiZe Custom Assets/WorldTypes")]
public class WorldTypes : ScriptableObject
{
    public string seed = "Toyota";
    public int sizeX = 8;
    public int sizeZ = 8;
    
    public LayerGen[] noiseLayers = new LayerGen[1];
    
}
[Serializable]
public class LayerGen
{
    public Vector3 offset;
    public SimplexNoiseGenerator noise;
    public string seed;
    public int noiseMapResolution = 256;
    [Header("noiseGen Settings")]
    public int octaves = 1;
    public int multiplier = 25;
    public float amplitute = 0.5f;
    public float lacunarity = 2f;
    public float persistance = 0.9f;
    public Vector2 sizeScale = Vector2.one;
    public BlendModes blendMode = BlendModes.Multiply;

    [Header("Clamp Settings")]
    public float min = 0f;
    public float max = 1f;
    public enum BlendModes
    {
        Multiply,
        Subtract,
        Add,
        Divide,
    }

    public float[,] getNoiseMap()
    {
        noise = new SimplexNoiseGenerator(seed);
        float[,] noiseMap = new float[noiseMapResolution,noiseMapResolution];
        for (int x = 0; x < noiseMapResolution; x++)
        {
            for (int z = 0; z < noiseMapResolution; z++)
            {
                float noiseFloat = noise.coherentNoise(x + offset.x, 0 + offset.y, z + offset.z, octaves, multiplier, amplitute, lacunarity, persistance);
                //Debug.Log(noiseFloat);
                noiseMap[x, z] = (noiseFloat+1)/2;
                //Debug.Log(noiseMap[x, z]);
            }
        }

        return noiseMap;
    }
}
