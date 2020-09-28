using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoiseMapDebugger : MonoBehaviour
{

    public WorldTypes noiseMapGenerator;
    float[,] noiseMap;
    Texture2D noise;
    public MeshRenderer mr;
    public Boolean updateTextureUpdating = true;
    // Start is called before the first frame update
    void Start()
    {
        noise = new Texture2D(noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution);
        

    }

    // Update is called once per frame
    void Update()
    {
    }
    public void updateTexture()
    {
        float[,] lastPixel = new float[noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution];
        for (int i = 0; i < noiseMapGenerator.noiseLayers.Count; i++)
        {
            LayerGen noiseGen = noiseMapGenerator.noiseLayers[i];
            noiseMap = new float[noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution];
            for (int x = 0; x < noiseMapGenerator.noiseLayers[0].noiseMapResolution; x++)
            {
                for (int z = 0; z < noiseMapGenerator.noiseLayers[0].noiseMapResolution; z++)
                {
                    noiseMap[x, z] = noiseGen.getNoiseMap(x, z);
                    if (i > 0)
                    {
                    switch (noiseGen.blendMode)
                    {
                        case LayerGen.BlendModes.Multiply:
                            noiseMap[x, z] = noiseMap[x, z] * lastPixel[x, z];

                            break;
                        case LayerGen.BlendModes.Subtract:
                            noiseMap[x, z] = noiseMap[x, z] - lastPixel[x, z];

                            break;
                        case LayerGen.BlendModes.Add:
                            noiseMap[x, z] = noiseMap[x, z] + lastPixel[x, z];

                            break;
                        case LayerGen.BlendModes.Divide:
                            noiseMap[x, z] = noiseMap[x, z] / lastPixel[x,z];
                            break;
                        default:
                            break;
                    }
                    }
                    noise.SetPixel(x, z, new Color(noiseMap[x, z], noiseMap[x, z], noiseMap[x, z]));
                    
                }
            }
            lastPixel = noiseMap;


        }
        noise.Apply();
        mr.material.SetTexture("_BaseMap", noise);
    }
    public float scaleBetween(float unscaledNum, float minAllowed, float maxAllowed, float min, float max)
    {
        return (maxAllowed - minAllowed) * (unscaledNum - min) / (max - min) + minAllowed;
    }
}
