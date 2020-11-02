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
			var arr = new Vector2[gridHeight];
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

				var pos = new Vector2(x, y);
				var neighbours = allNeighboursOf(pos);

				//Go through all neighbours and find the one with the lowest distance
				int min;
				var minDist = 0;
				for (var i = 0; i < neighbours.length; i++)
				{
					var n = neighbours[i];
					var dist = dijkstraGrid[n.x][n.y] - dijkstraGrid[pos.x][pos.y];

					if (dist < minDist)
					{
						min = n;
						minDist = dist;
					}
				}

				//If we found a valid neighbour, point in its direction
				if (min != null)
				{
					flowField[x][y] = min.minus(pos).normalize();
				}
			}
		}
	}

    private object allNeighboursOf(Vector2 pos)
    {
        throw new NotImplementedException();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
