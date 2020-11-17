using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    private Vector2[] goals;
    public class Cell
    {
        internal bool unpassable;
        public Vector2 position;
        public float distance;
        private Vector2 vector2;

        public Cell(Vector2 vector2)
        {
            this.vector2 = vector2;
        }
    }




    public class Grid
    {
        public Cell[] cells;
        private int width, lenght;

        private Grid() { }
        public Grid(int width, int lenght)
        {
            this.width = width;
            this.lenght = lenght;
        }
        public void Create()
        {
            cells = new Cell[width * lenght];
            for (int p = 0; p < width; p++)
            {
                for (int s = 0; s < lenght; s++)
                {
                    int index = p * lenght + s;
                    var cell = new Cell(new Vector2(p, s));

                    cells[index] = cell;
                }
            }

        }
        public Cell[] GetNeighbours(Cell current)
        {
            var result = new Cell[4];
            int x = (int)current.position.x;
            int y = (int)current.position.y;

            var indices = new int[]
            {
                x * lenght + (y + 1),
                (x + 1) * lenght + y,
            x * lenght + (y - 1),
            (x - 1) * lenght + y
            };

            if (y < lenght - 1)
                result[0] = cells[indices[0]];
            if (x < width - 1)
                result[1] = cells[indices[1]];
            if (y > 0)
                result[2] = cells[indices[2]];
            if (x > 0)
                result[3] = cells[indices[3]];
            return result;
        }

    }
    public class Algorithm
    {
        private Grid grid;
        private Vector2[] goal;

        private Algorithm() { }

        public Algorithm(Grid grid, Vector2[] goal)
        {
            this.grid = grid;
            this.goal = goal;
        }

        public void CreateCostField()
        {
            var marked = new List<Cell>();
            var cells = grid.cells;
            for (int i = 0; i < goal.Length; i++)
            {
                var goalCell = cells.First(c => cells.position == goal[i]);
                goalCell.distance = 0;

                marked.Add(goalCell);
            }
            if (goal == null || goal.Length < 1)
            {
                Debug.LogError("No goal!");
                return;
            }
            while (marked.Count < cells.Length)
            {
                for (int i = 0; i < marked.Count; i++)
                {
                    if (marked[i].unpassable)
                        continue;
                    var neighbours = grid.GetNeighbours(marked[i]);
                    for (int j = 0; j < 8; j++)
                    {
                        var cur = neighbours[j];
                        if (cur == null || marked.Contains(cur))
                            continue;
                        cur.distance = marked[i].distance;
                        cur.distance += (cur.position - marked[i].position).magnitude;

                        marked.Add(cur);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}

