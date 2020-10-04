using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class World : MonoBehaviour
{
    private static World _main;
    public static World main
    {
        get { return _main; }
    }

    public static int chunkSize = 32;
    public static GameObject world;
    public GameObject Player;

    public int viewDistance = 5;
    public Material terrianMaterial;

    private Dictionary<ChunkPoint, Chunk> _chunks = new Dictionary<ChunkPoint, Chunk>();
    private Dictionary<ChunkPoint, GameObject> _goCache = new Dictionary<ChunkPoint, GameObject>();


    public void Awake()
    {
        if (_main == null)
            _main = this;
    }
    public enum LODLEVELS
    {
        LOD0,
        LOD1,
        LOD2,
        LOD3
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
    }

    private void Start()
    {
        
    }
}
