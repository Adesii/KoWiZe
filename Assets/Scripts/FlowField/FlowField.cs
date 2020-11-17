using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    public FlowFieldPoint[][] flowField;
    public int gridWidth;
    public int gridHeight;
    public Vector3 posit;

    // Start is called before the first frame update
    void Start()
    {
        posit = transform.position;
        generateFlowField();
        print(flowField);
    }
    public class FlowFieldPoint
    {
        public Vector2 direction;
        public int cost;
        public Vector2 position;
        public Vector3 wpp;
        public FlowFieldPoint(Vector2 pos,Vector3 wp)
        {
            position = pos;
            wpp =new Vector3(wp.x+pos.x,wp.y,wp.z+pos.y);

        }
        public FlowFieldPoint()
        {
        }
        public FlowFieldPoint[] getNeighbours(ref FlowFieldPoint[][] reference)
        {
            List<FlowFieldPoint> temp = new List<FlowFieldPoint>();
            if (position.x - 1 > 0)
                temp.Add(reference[(int)position.x - 1][(int)position.y]);
            if (position.x + 1 < reference.Length)
                temp.Add(reference[(int)position.x + 1][(int)position.y]);
            if (position.y + 1 < reference[0].Length)
                temp.Add(reference[(int)position.x][(int)position.y + 1]);
            if (position.y - 1 > 0)
                temp.Add(reference[(int)position.x][(int)position.y - 1]);
            return temp.ToArray();
        }
    }

    void generateFlowField()
    {
        int x, y;

        //Generate an empty grid, set all places as Vector2.zero, which will stand for no good direction
        flowField = new FlowFieldPoint[gridWidth][];
        for (x = 0; x < gridWidth; x++)
        {
            FlowFieldPoint[] arr = new FlowFieldPoint[gridHeight];
            for (y = 0; y < gridHeight; y++)
            {
                arr[y] = new FlowFieldPoint(new Vector2(x, y),posit);
            }
            flowField[x] = arr;
        }

        //for each grid square
        for (x = 0; x < gridWidth; x++)
        {
            for (y = 0; y < gridHeight; y++)
            {

                Vector2 pos = new Vector2(x, y);
                FlowFieldPoint[] neighbours = flowField[x][y].getNeighbours(ref flowField);

                //Go through all neighbours and find the one with the lowest distance
                FlowFieldPoint min = new FlowFieldPoint();
                float minDist = 0;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    FlowFieldPoint n = neighbours[i];
                    float dist = Vector2.Distance(flowField[(int)n.position.x][(int)n.position.y].position, flowField[(int)pos.x][(int)pos.y].position);

                    if (dist < minDist)
                    {
                        min = n;
                        minDist = dist;
                    }
                }

                //If we found a valid neighbour, point in its direction
                if (min != null)
                {
                    pos.Normalize();
                    flowField[x][y].direction = -pos;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        foreach (FlowFieldPoint[] item in flowField)
        {
            foreach (FlowFieldPoint point in item)
            {
                Gizmos.DrawLine(point.wpp, point.wpp + (new Vector3(point.direction.x,0,point.direction.y)));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
