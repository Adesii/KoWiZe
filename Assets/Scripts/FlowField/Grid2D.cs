using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    // Start is called before the first frame update

    public int size = 10;
    public Grid grid;
    public Algorithm rith;
    public Vector3 goal;
    void Start()
    {
        grid = new Grid(10, 10);
        grid.Create();
        rith = new Algorithm(grid, goal);
        rith.GenerateVectorFields();
        rith.CreateCostField();


    }
    public class Cell
    {
        internal bool unpassable;
        public Vector2 position;
        public float distance;
        private Vector2 vector2;
        internal Vector2 direction;

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

        public Cell[] GetMooreNeighbours(Cell current)
        {
            var result = new Cell[8];

            int x = (int)current.position.x;
            int y = (int)current.position.y;

            var indices = new int[]
            {
            x * lenght + (y + 1),
            (x + 1) * lenght + y,
            x * lenght + (y - 1),
            (x - 1) * lenght + y,
            (x - 1) * lenght + (y + 1),
            (x + 1) * lenght + (y + 1),
            (x - 1) * lenght + (y - 1),
            (x + 1) * lenght + (y - 1)
            };

            if (y < lenght - 1 && x > 0)
                result[4] = cells[indices[4]];
            if (y < lenght - 1 && x < width - 1)
                result[5] = cells[indices[5]];
            if (y > 0 && x > 0)
                result[6] = cells[indices[6]];
            if (y > 0 && x < width - 1)
                result[7] = cells[indices[7]];

            return result;
        }
        public Cell FindCell(Vector2 pos)
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (x >= 0 && x < width && y >= 0 && y < lenght)
            {
                int index = x * lenght + y;
                var cell = cells[index];
                return cell;
            }

            return null;
        }
    }
    public class Algorithm
    {
        public Grid grid;
        private Vector2 goal;
        private Cell goalCellVector;

        private Algorithm() { }

        public Algorithm(Grid grid, Vector2 goal)
        {
            this.grid = grid;
            this.goal = goal;
        }

        public void CreateCostField()
        {
            var marked = new List<Cell>();
            var cells = grid.cells;
            goalCellVector = grid.FindCell(goal);
            var goalCell = cells.First(c => c.position == goalCellVector.position);
            goalCell.distance = 0;

            marked.Add(goalCell);
            if (goal == null)
            {
                Debug.LogError("No goal!");
                return;
            }
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].unpassable)
                    continue;
                var neighbours = grid.GetNeighbours(cells[i]);
                for (int j = 0; j < 4; j++)
                {
                    var cur = neighbours[j];
                    if (cur == null || cells.Contains(cur))
                        continue;
                    cur.distance = cells[i].distance;
                    cur.distance += (cur.position - cells[i].position).magnitude;

                    marked.Add(cur);
                }
            }
        }
        public void GenerateVectorFields()
        {
            for (int i = 0; i < grid.cells.Length; i++)
            {
                var cur = grid.cells[i];
                var neighbours = grid.GetNeighbours(cur);

                float left, right, up, down;
                left = right = up = down = cur.distance;

                if (neighbours[0] != null && !neighbours[0].unpassable) up = neighbours[0].distance;
                if (neighbours[1] != null && !neighbours[1].unpassable) right = neighbours[1].distance;
                if (neighbours[2] != null && !neighbours[2].unpassable) down = neighbours[2].distance;
                if (neighbours[3] != null && !neighbours[3].unpassable) left = neighbours[3].distance;


                float x = left - right;
                float y = down - up;

                cur.direction = new Vector2(x, y);
                cur.direction.Normalize();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (rith != null)
            foreach (var item in rith.grid.cells)
            {
                Debug.Log(item.direction);
                Gizmos.DrawLine(item.position, item.position + item.direction);
            }
    }
    // Update is called once per frame
    void Update()
    {

    }

}


