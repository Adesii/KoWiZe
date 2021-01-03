using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class computeShaderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public WorldTypes typeOfWorld;

    public int numbOfThreads = 10;

    public int textureSize = 256;
    [Range(0, 16)]
    public int octaves = 1;
    [Range(0, 250)]
    public int multiplier = 25;
    [Range(0, 10)]
    public float amplitude = 0.5f;
    [Range(0, 10)]
    public float lacunarity = 2;
    [Range(0, 10)]
    public float persistence = 0.9f;


    void Start()
    {
        computeShader.SetFloat("resolution", textureSize);
    }
    void Update()
    {

        computeShader.SetVector("worldpos", transform.position);

        //computeShader.Dispatch(computeShader.FindKernel("CSMain"), renderTexture.width / 10, renderTexture.height / 10, 1);
    }
    public void GPUcompute()
    {
        int si = 0;
        worldData ws = prepareLayerSettings(typeOfWorld);
        List<meshData> w = new List<meshData>();
        int res = textureSize + 1 * textureSize + 1;
        for (int i = 0; i < textureSize*textureSize; i++)
        {
            w.Add(new meshData
            {
                vert = new Vector3(),
                normal = new Vector3(),
                uv = new Vector2(),
            });
            si = (sizeof(float) * 3 * 2) + (sizeof(float) * 2);
        }

        ComputeBuffer comp = new ComputeBuffer(w.Count, si);
        ComputeBuffer tris = new ComputeBuffer(w.Count, sizeof(int) * 6,ComputeBufferType.Append);
        comp.SetData(w);

        computeShader.SetInts("octa",ws.octa);
        computeShader.SetFloats("mult", ws.mult);
        computeShader.SetFloats("ampl", ws.ampl);
        computeShader.SetFloats("lacu", ws.lacu);
        computeShader.SetFloats("pers", ws.pers);
        computeShader.SetFloats("sizeScalex", ws.sizeScalex);
        computeShader.SetFloats("sizeScalez", ws.sizeScalez);
        computeShader.SetFloats("MultiplicationOfFinal", ws.MultiplicationOfFinal);
        computeShader.SetFloats("subtraction", ws.subtraction);
        computeShader.SetInts("blendmode", ws.blendmode);
        computeShader.SetFloats("offsetX", ws.offsetX);
        computeShader.SetFloats("offsetY", ws.offsetY);
        computeShader.SetFloats("offsetZ", ws.offsetZ);
        computeShader.SetFloats("min", ws.min);
        computeShader.SetFloats("max", ws.max);
        computeShader.SetFloats("minHeight", ws.minHeight);
        computeShader.SetFloats("maxHeight", ws.maxHeight);

        computeShader.SetBuffer(0, "mesh", comp);
        computeShader.SetBuffer(0, "tris", tris);
        computeShader.Dispatch(0, w.Count / numbOfThreads, 1, 1);

        meshData[] meshDatas = { };
        _tri[] tri = {};
        comp.GetData(meshDatas);
        tris.GetData(tri);


        Mesh m = new Mesh();
        List<Vector3> vertss = new List<Vector3>();
        List<int> triss = new List<int>();
        foreach (var item in tri)
        {
            triss.Add(item.tris1);
            triss.Add(item.tris2);
            triss.Add(item.tris3);
            triss.Add(item.tris4);
            triss.Add(item.tris5);
            triss.Add(item.tris6);
        }
        foreach (var item in meshDatas)
        {
            vertss.Add(item.vert);
            Debug.DrawLine(item.vert, Vector3.up + item.vert * 100, Color.white, 10f);
        }
        m.vertices = vertss.ToArray();
        m.triangles = triss.ToArray();
        Debug.Log("Size of Verts: " + vertss.Count+" Count of Tris: "+triss.Count);
        
        GetComponent<MeshFilter>().mesh = m;

    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Compute"))
        {
            GPUcompute();
        }
    }
    private worldData prepareLayerSettings(WorldTypes worldType)
    {
        int res = (textureSize + 1 * textureSize + 1);
        worldData world = new worldData
        {
            numberoflayers = worldType.noiseLayers.Count,
            noiseLayeredMap = new float[res],
            lastnoiseLayeredMap = new float[res],

            octa = new int[worldType.noiseLayers.Count],
            mult = new float[worldType.noiseLayers.Count],
            ampl = new float[worldType.noiseLayers.Count],
            lacu = new float[worldType.noiseLayers.Count],
            pers = new float[worldType.noiseLayers.Count],
            sizeScalex = new float[worldType.noiseLayers.Count],
            sizeScalez = new float[worldType.noiseLayers.Count],
            MultiplicationOfFinal = new float[worldType.noiseLayers.Count],
            subtraction = new float[worldType.noiseLayers.Count],
            blendmode = new int[worldType.noiseLayers.Count],
            offsetX = new float[worldType.noiseLayers.Count],
            offsetY = new float[worldType.noiseLayers.Count],
            offsetZ = new float[worldType.noiseLayers.Count],
            min = new float[worldType.noiseLayers.Count],
            max = new float[worldType.noiseLayers.Count],
            minHeight = new float[worldType.noiseLayers.Count],
            maxHeight = new float[worldType.noiseLayers.Count]

        };
        for (int i = 0; i < worldType.noiseLayers.Count; i++)
        {
            LayerGen currWorld = worldType.noiseLayers[i];
            world.octa[i] = currWorld.octaves;
            world.mult[i] = currWorld.multiplier;
            world.ampl[i] = currWorld.amplitute;
            world.lacu[i] = currWorld.lacunarity;
            world.pers[i] = currWorld.persistance;
            world.sizeScalex[i] = currWorld.sizeScalex;
            world.sizeScalez[i] = currWorld.sizeScalez;
            world.blendmode[i] = (int)currWorld.blendMode;
            world.MultiplicationOfFinal[i] = currWorld.MultiplicationOfFinal;
            world.subtraction[i] = currWorld.subtraction;
            world.offsetX[i] = currWorld.offset.x;
            world.offsetY[i] = currWorld.offset.y;
            world.offsetZ[i] = currWorld.offset.z;
            world.min[i] = currWorld.min;
            world.max[i] = currWorld.max;
        }
        return world;
    }

    struct worldData
    {
        public int numberoflayers;

        public float[] noiseLayeredMap;
        public float[] lastnoiseLayeredMap;



        public int[] octa;
        public float[] mult;
        public float[] ampl;
        public float[] lacu;
        public float[] pers;
        public float[] sizeScalex;
        public float[] sizeScalez;
        public float[] MultiplicationOfFinal;
        public float[] subtraction;
        public int[] blendmode;
        public float[] offsetX;
        public float[] offsetY;
        public float[] offsetZ;
        public float[] min;
        public float[] max;
        public float[] minHeight;
        public float[] maxHeight;

    };
   
    struct meshData
    {
        public Vector3 vert;
        public Vector3 normal;
        public Vector2 uv;
    }
    struct _tri
    {
        public int tris1, tris2, tris3, tris4, tris5, tris6;
    }
}
