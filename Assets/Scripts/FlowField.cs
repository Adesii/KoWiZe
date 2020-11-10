using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    private FlowFieldPoint[][] flowField;
    private int gridWidth;
    private int gridHeight;

    // Start is called before the first frame update
    void Start()
    {
    }
    class FlowFieldPoint
    {
        public Vector2 direction;
        public int cost;
        public Vector2 position;
        public FlowFieldPoint(Vector2 pos)
        {
            position = pos;
            
        }
        public FlowFieldPoint()
        {
        }
        public FlowFieldPoint[] getNeighbours(ref FlowFieldPoint[][] reference)
        {
            return null;
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
				arr[y] = new FlowFieldPoint(new Vector2(x,y));
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

    private  Vector2[] allNeighboursOf(Vector2 pos)
    {
        throw new NotImplementedException();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
