using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
[CreateAssetMenu(fileName = "WorldType",menuName ="KoWiZe Custom Assets/WorldTypes")]
public class WorldTypes : ScriptableObject
{
    public string seed = "Toyota";
    public int sizeX = 8;
    public int sizeZ = 8;
   
    
    [Reorderable]
    public List<LayerGen> noiseLayers = new List<LayerGen>(1);
    
}


[Serializable]
public class LayerGen
{
    public Vector3 offset;
    public SimplexNoiseGenerator noise;
    public string seed;
    [Header("noiseGen Settings")]
    [Range(1, 10)]
    public int octaves = 1;
    public int multiplier = 25;
    public float amplitute = 0.5f;
    public float lacunarity = 2f;
    public float persistance = 0.9f;
    public AnimationCurve heightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [Header("SizeScale")]
    public float sizeScalex =1f;
    public float sizeScalez = 1f;
    [Header("BlendMode")]
    public BlendModes blendMode = BlendModes.Multiply;

    [Header("Clamp Settings")]
    public float min = 0f;
    public float max = 1f;
    public float MultiplicationOfFinal = 1f;
    public float subtraction = 0f;

    public enum BlendModes
    {
        Multiply,
        Subtract,
        Add,
        Divide,
        Mask,
    }

    public float getNoiseMap(float ChunkX,float ChunkZ)
    {
        noise = new SimplexNoiseGenerator(seed);
        float noiseFloat = noise.coherentNoise((offset.x+ ChunkX) * sizeScalex, 0 + offset.y, (offset.z+ ChunkZ) * sizeScalez, octaves, multiplier, amplitute, lacunarity, persistance);
        //Debug.Log(noiseFloat);
        //Debug.Log(noiseMap[x, z]);
        
        noiseFloat = (noiseFloat + 1) / 2;
        noiseFloat = heightCurve.Evaluate(noiseFloat);
        noiseFloat *= MultiplicationOfFinal;
        
        return noiseFloat -subtraction;
    }
}
