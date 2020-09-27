using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapDebugger : MonoBehaviour
{

    public WorldTypes noiseMapGenerator;
    float[,] noiseMap;
    Texture2D noise;
    public MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        noise = new Texture2D(noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution);
        InvokeRepeating("updateTexture", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void updateTexture()
    {
        float[,] lastPixel = new float[noiseMapGenerator.noiseLayers[0].noiseMapResolution, noiseMapGenerator.noiseLayers[0].noiseMapResolution];
        for (int i = 0; i < noiseMapGenerator.noiseLayers.Length; i++)
        {
            LayerGen noiseGen = noiseMapGenerator.noiseLayers[i];
            noiseMap = noiseGen.getNoiseMap();
            for (int x = 0; x < noiseMapGenerator.noiseLayers[0].noiseMapResolution; x++)
            {
                for (int z = 0; z < noiseMapGenerator.noiseLayers[0].noiseMapResolution; z++)
                {
                    if (i > 0)
                    {
                    switch (noiseGen.blendMode)
                    {
                        case LayerGen.BlendModes.Multiply:
                            noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] * lastPixel[x, z]);

                            break;
                        case LayerGen.BlendModes.Subtract:
                            noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] - lastPixel[x, z]);

                            break;
                        case LayerGen.BlendModes.Add:
                            noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] + lastPixel[x, z]);

                            break;
                        case LayerGen.BlendModes.Divide:
                            noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] / lastPixel[x,z]);
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
