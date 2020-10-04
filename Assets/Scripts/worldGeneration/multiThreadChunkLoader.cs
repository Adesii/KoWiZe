using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class multiThreadChunkLoader : MonoBehaviour
{
    public NativeArray<Vector3> verts;
    public Vector3[] m_verts;
    public int res = 128;
    public int chunkAm = 8;

    private void Start()
    {
        verts = new NativeArray<Vector3>(chunkAm * chunkAm * res, Allocator.Persistent);
        NativeArray<JobHandle> chunkJobs = new NativeArray<JobHandle>(chunkAm * chunkAm, Allocator.Persistent);
        NativeArray<int> playerRes = new NativeArray<int>(chunkAm * chunkAm, Allocator.Persistent);
        for (int i = 0; i < playerRes.Length; i++)
        {
            playerRes[i] = 1;
        }

        chunkManagerJob manager = new chunkManagerJob
        {
            chunkAmount = chunkAm,
            chunkJobs = chunkJobs,
            lowestResulotion = res,
            playerResolution = playerRes,
            verts = verts
        };
        m_verts = new Vector3[chunkAm * chunkAm * res];
        JobHandle j= manager.Schedule(verts.Length,32);
        j.Complete();
        manager.verts.CopyTo(m_verts);
        verts.Dispose();
        Debug.Log(m_verts.Length);
        chunkJobs.Dispose();
        playerRes.Dispose();
        
        
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 item in m_verts)
        {
            Gizmos.DrawSphere(item, 0.1f);
        }
    }
    private void OnGUI()
    {
        //GUI.Label(Rect.PointToNormalized());
    }
    public struct chunkManagerJob : IJobParallelFor
    {
        [ReadOnly] public int chunkAmount;
        [ReadOnly] public int lowestResulotion;
        [ReadOnly] public NativeArray<int> playerResolution;
        public NativeArray<JobHandle> chunkJobs;
        public NativeArray<Vector3> verts;
        public void Execute(int index)
        {
            float res = lowestResulotion;
            float x = index % res;
            float y = index / res;
            verts[index] = new Vector3(x,Mathf.PerlinNoise(x,y)*10,y);
        }
    }

}
