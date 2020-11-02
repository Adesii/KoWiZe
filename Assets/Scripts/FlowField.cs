using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    private Vector2[][] flowField;
    private int gridWidth;
    private int gridHeight;

    // Start is called before the first frame update
    void Start()
    {
    }


    void generateFlowField()
    {
        int x, y;

        //Generate an empty grid, set all places as Vector2.zero, which will stand for no good direction
        flowField = new Vector2[gridWidth][];
		for (x = 0; x < gridWidth; x++)
		{
			Vector2[] arr = new Vector2[gridHeight];
			for (y = 0; y < gridHeight; y++)
			{
				arr[y] = Vector2.zero;
			}
			flowField[x] = arr;
		}

		//for each grid square
		for (x = 0; x < gridWidth; x++)
		{
			for (y = 0; y < gridHeight; y++)
            {

                Vector2 pos = new Vector2(x, y);
                Vector2[] neighbours = allNeighboursOf(pos);

                //Go through all neighbours and find the one with the lowest distance
                Vector2 min = new Vector2();
                float minDist = 0;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    Vector2 n = neighbours[i];
                    float dist = Vector2.Distance(flowField[(int)n.x][(int)n.y], flowField[(int)pos.x][(int)pos.y]);

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
                    flowField[x][y] = -pos;
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
