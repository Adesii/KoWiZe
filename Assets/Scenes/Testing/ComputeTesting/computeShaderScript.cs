using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class computeShaderScript : MonoBehaviour
{

    // Start is called before the first frame update
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public WorldTypes typeOfWorld;

    public const int NumberOfThreads = 8;


    public int chunkRes = 256;
    public int chunkSize = 256;

    List<Vector3> vertss;

    public bool autoRefresh = false;
    public float refreshTimer = 1f;
    float counter = 0f;
    void Update()
    {
        computeShader.SetVector("worldpos", transform.position / chunkRes);
        computeShader.SetFloat("chunkRes", chunkRes);
        computeShader.SetFloat("chunkSize", chunkSize);


        if (counter > refreshTimer && autoRefresh)
        {
            counter = 0f;
            GPUcompute();
        }
        counter += 1f * Time.deltaTime;
        //computeShader.Dispatch(computeShader.FindKernel("CSMain"), renderTexture.width / 10, renderTexture.height / 10, 1);
    }
    public void GPUcompute()
    {
        int si = 0;
        worldData ws = prepareLayerSettings(typeOfWorld);
        List<meshData> w = new List<meshData>();
        for (int i = 0; i < chunkRes * 2; i++)
        {
            for (int j = 0; j < chunkRes * 2; j++)
            {
                w.Add(new meshData
                {
                    vert = new Vector3(-1f, 0, -1f),
                    normal = new Vector3(0, 0, 0),
                    uv = new Vector2(0, 0),
                });
                si = (sizeof(float) * 3 * 2) + (sizeof(float) * 2);
            }

        }

        ComputeBuffer comp = new ComputeBuffer(w.Count, si, ComputeBufferType.Structured);
        ComputeBuffer tris = new ComputeBuffer(w.Count, sizeof(int) * 6, ComputeBufferType.Append);
        comp.SetData(w.ToArray());

        computeShader.SetInts("octa", ws.octa);
        computeShader.SetFloats("multi", ws.mult);
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
        computeShader.SetFloat("numberoflayers", 1);
        computeShader.SetBuffer(0, "mesh", comp);
        computeShader.SetBuffer(0, "tris", tris);
        computeShader.Dispatch(computeShader.FindKernel("CSMain"), NumberOfThreads, NumberOfThreads, 1);

        meshData[] meshDatas = new meshData[w.Count];
        _tri[] tri = new _tri[w.Count];
        comp.GetData(meshDatas);
        tris.GetData(tri);


        Mesh m = new Mesh();
        vertss = new List<Vector3>();
        List<int> triss = new List<int>();
        for (int i = 0; i < tri.Length; i++)
        {
            if (i < w.Count)
            {
                var item = tri[i];
                var vert = meshDatas[i];
                vertss.Add(vert.vert);
                triss.Add(item.tris1);
                triss.Add(item.tris2);
                triss.Add(item.tris3);
                triss.Add(item.tris4);
                triss.Add(item.tris5);
                triss.Add(item.tris6);

            }
        }

        m.vertices = vertss.ToArray();
        m.triangles = triss.ToArray();
        Debug.Log("Size of Verts: " + vertss.Count + " Count of Tris: " + triss.Count);

        GetComponent<MeshFilter>().mesh = m;
        comp.Dispose();
        tris.Dispose();
        //vertss.Clear();

    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Compute"))
        {
            GPUcompute();
        }
    }
    private void OnDrawGizmos()
    {
        if (vertss != null)
            foreach (var item in vertss)
            {
                //Gizmos.DrawLine(item, item + Vector3.up);
                if (item != new Vector3(-1f, 0, -1f))
                    Gizmos.DrawCube(item, Vector3.one * 5);
            }
    }
    private worldData prepareLayerSettings(WorldTypes worldType)
    {
        int res = (chunkRes * chunkRes);
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
