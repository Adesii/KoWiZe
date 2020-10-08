using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using WorldGenJobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Runtime.ConstrainedExecution;
using static LayerGen;
using System.Runtime.CompilerServices;

public class World : MonoBehaviour
{
    private static World _main;
    public static World main
    {
        get { return _main; }
    }

    public int LODRadius = 1;

    [ReadOnly]
    public static int chunkSize = 128;
    public static GameObject world;
    public GameObject Player;

    public bool liveEditb = true;
    public float updateRate = 2f;
    public Material terrianMaterial;
    public WorldTypes typeOfWorld;
    public LODLEVELS defaultLOD;

    private jobGenerationManager jobManager;
    public static Dictionary<ChunkPoint, Chunk> _chunks = new Dictionary<ChunkPoint, Chunk>();
    public static List<Chunk> _activeChunks = new List<Chunk>();
    public Vector2 currPlayerChunk;
    public float lodPow= 2;



    public void Awake()
    {
        if (_main == null)
            _main = this;
        jobManager = gameObject.AddComponent<jobGenerationManager>();
        jobManager.defaultMaterial = terrianMaterial;
        jobManager.initalizeManager(typeOfWorld);


    }
    public enum LODLEVELS
    {
        LOD0,
        LOD1,
        LOD2,
        LOD3,
        LOD4
    }
    [Serializable]
    public struct ChunkPoint
    {
        private readonly int _x;
        private readonly int _z;

        public ChunkPoint(int x, int z)
        {
            this._x = x;
            this._z = z;
        }

        public int X
        {
            get { return _x; }
        }

        public int Z
        {
            get { return _z; }
        }

        public ChunkPoint Move(Vector2 offset)
        {
            return new ChunkPoint(_x + (int)offset.x, _z + (int)offset.y);
        }
        public void displayChunk()
        {
            Debug.DrawLine(new Vector3(_x * chunkSize, 0, _z * chunkSize), new Vector3(_x * chunkSize, 10, _z * chunkSize), Color.yellow, 1000f);
        }

        public static Vector2 operator -(ChunkPoint a, ChunkPoint b)
        {
            return new Vector2(a._x - b._x, a._z - b._z);
        }

        public static Vector2 operator +(ChunkPoint a, ChunkPoint b)
        {
            return new Vector2(a._x + b._x, a._z + b._z);
        }

        public static bool operator ==(ChunkPoint a, ChunkPoint b)
        {
            return a._x == b._x && a._z == b._z;
        }

        public static bool operator !=(ChunkPoint a, ChunkPoint b)
        {
            return a._x != b._x || a._z != b._z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkPoint)) return false;
            var c = (ChunkPoint)obj;

            return c.X == X && c.Z == Z;

        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + _x;
            hash = hash * 31 + _z;
            return hash;
        }

        public override string ToString()
        {
            return "(" + _x + ", " + _z + ")";
        }
        public Vector2 toVector2()
        {
            return new Vector2(_x, _z);
        }
    }

    private void Start()
    {
        GenerateChunks();
        StartCoroutine(liveEdit());
        StartCoroutine(updateLOD());
    }

    public IEnumerator updateLOD()
    {
        while (true)
        {
            for (int x = -LODRadius; x < LODRadius; x++)
            {
                for (int z = -LODRadius; z < LODRadius; z++)
                {

                    ChunkPoint chunk = new ChunkPoint((int)((Player.transform.position.x / chunkSize) + (x) + ((typeOfWorld.sizeX) / 2f)), (int)((Player.transform.position.z / chunkSize) + (z) + ((typeOfWorld.sizeZ) / 2f)));
                    ChunkPoint playerChunk = new ChunkPoint((int)((Player.transform.position.x / chunkSize) + ((typeOfWorld.sizeX) / 2f)), (int)((Player.transform.position.z / chunkSize) + ((typeOfWorld.sizeZ) / 2f)));
                    currPlayerChunk = new Vector2(playerChunk.X, playerChunk.Z);
                    LODLEVELS lodLevel = (LODLEVELS)Mathf.Pow(Vector2.Distance(chunk.toVector2(), playerChunk.toVector2()),lodPow);
                    
                    if (_chunks.ContainsKey(chunk) && _chunks[chunk].LOD != lodLevel &&  chunk.X > 0 && chunk.Z > 0 && chunk.X <= typeOfWorld.sizeZ && chunk.Z <= typeOfWorld.sizeX )
                    {
                        if (!_chunks[chunk].hasMesh(lodLevel))
                        {
                        jobManager.GenerateChunkAt(chunk, lodLevel);
                        }
                        else
                        {
                            _chunks[chunk].updateMesh(lodLevel);
                        }
                            /*if(!_activeChunks.Contains(_chunks[chunk]) &&lodLevel != LODLEVELS.LOD3)
                            {
                                _activeChunks.Add(_chunks[chunk]);
                            }
                            */
                        

                    }
                }
            }
            yield return new WaitForSeconds(updateRate);
        }

    }

    public void regenerate()
    {
        foreach (var item in _chunks)
        {
            item.Value.Dispose();
            Destroy(item.Value.chunk);
        }
        _chunks.Clear();
        for (int x = 0; x < typeOfWorld.sizeX; x++)
        {
            for (int z = 0; z < typeOfWorld.sizeX; z++)
            {
                ChunkPoint newCP = new ChunkPoint(x, z);
                jobManager.GenerateChunkAt(newCP, defaultLOD);
            }
        }
    }
    public IEnumerator liveEdit()
    {
        while (liveEditb)
        {


            foreach (var item in _chunks)
            {
                item.Value.Dispose();
                Destroy(item.Value.chunk);
            }
            _chunks.Clear();
            for (int x = 0; x < typeOfWorld.sizeX; x++)
            {
                for (int z = 0; z < typeOfWorld.sizeX; z++)
                {
                    ChunkPoint newCP = new ChunkPoint(x, z);
                    jobManager.GenerateChunkAt(newCP, defaultLOD);
                }
            }


            yield return new WaitForSeconds(updateRate);
        }
    }
    private void GenerateChunks()
    {
        for (int x = 0; x < typeOfWorld.sizeX; x++)
        {
            for (int z = 0; z < typeOfWorld.sizeX; z++)
            {
                ChunkPoint newCP = new ChunkPoint(x, z);

                jobManager.GenerateChunkAt(newCP, defaultLOD);
            }
        }
    }

}
