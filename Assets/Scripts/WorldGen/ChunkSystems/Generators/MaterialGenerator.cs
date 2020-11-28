using System.Collections;
using UnityEngine;

public static class MaterialGenerator
{


    public static Material GenerateMaterial(Material baseShader, WorldTypes typeOfWorld, float[] heightmap, int chunkSize, int textureSizeMultiplier = 1)
    {
        Material mat = new Material(baseShader);
        int chunkRess = (heightmap.Length / chunkSize-1);
        Texture2D mainText = new Texture2D((chunkRess), (chunkRess));
        Texture2D roughText = new Texture2D((chunkRess), (chunkRess));
        Texture2D normalText = new Texture2D((chunkRess), (chunkRess));

        Debug.Log(chunkSize);
        Debug.Log(heightmap.Length);

        Debug.Log(chunkRess);
        for (int x = 0; x < chunkRess; x++)
        {
            for (int y = 0; y < chunkRess; y++)
            {
                Color main = new Color();
                Color norm = new Color();
                Color rough = new Color();
                int index = ((x * chunkRess) + y);
                if (index >= heightmap.Length) break;
                for (int i = 0; i < typeOfWorld.TexturesForTerrain.textureList.Count; i++)
                {
                    main = typeOfWorld.TexturesForTerrain.textureList[i].mainTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);
                    norm = typeOfWorld.TexturesForTerrain.textureList[i].normalTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);
                    rough = typeOfWorld.TexturesForTerrain.textureList[i].roughTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);
                    if (i + 1 >= typeOfWorld.TexturesForTerrain.textureList.Count) continue;
                    if (heightmap[index] > typeOfWorld.TexturesForTerrain.textureList[i].height) continue;
                    Color main2 = typeOfWorld.TexturesForTerrain.textureList[i + 1].mainTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);
                    Color norm2 = typeOfWorld.TexturesForTerrain.textureList[i + 1].normalTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);
                    Color rough2 = typeOfWorld.TexturesForTerrain.textureList[i + 1].roughTexture.GetPixelBilinear(x / (float)chunkSize, y / (float)chunkSize);

                    float pos = (heightmap[index] - typeOfWorld.TexturesForTerrain.textureList[i].height);
                    main = Color.Lerp(main, main2, Mathf.Clamp01(pos));

                    norm = Color.Lerp(norm, norm2, Mathf.Clamp01(pos));
                    rough = Color.Lerp(rough, rough2, Mathf.Clamp01(pos));

                    break;
                }

                mainText.SetPixel(x, y, main);
                normalText.SetPixel(x, y, norm);
                roughText.SetPixel(x, y, rough);
            }
        }
        mainText.filterMode = FilterMode.Trilinear;
        normalText.filterMode = FilterMode.Trilinear;
        roughText.filterMode = FilterMode.Trilinear;
        mainText.Apply();
        normalText.Apply();
        roughText.Apply();
        mat.SetTexture("texture_main", mainText);
        mat.SetTexture("texture_normal", normalText);
        mat.SetTexture("texture_rough", roughText);
        return mat;
    }
}
