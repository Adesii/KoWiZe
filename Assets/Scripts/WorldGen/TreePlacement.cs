using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlacement : MonoBehaviour
{

    public GameObject TreePrefab;
    public List<GameObject> placeableModels;
    public List<GameObject> trees = new List<GameObject>();
    public int maxPlacedTrees = 200;
    public int maxForestSize = 10;
    public int minForestSize = 2;
    [Range(0, 1)]
    public float successProcent = 0.50f;

    public bool Regenerate = false;
    public bool ClearList = false;
    public float heightLimit = 14f;
    public float minHeight = 1.5f;
    Vector3 absoluteStartSpot;

    Coroutine co;
    [SerializeField]
    GameObject TreeParent;


    List<Vector2> treePositions;


    // Start is called before the first frame update
    private void Awake()
    {
        TreeParent = new GameObject();
        TreeParent.transform.parent = transform;
        TreePrefab = GameController.TreePrefab;
        placeableModels = GameController.PlaceableModels;
        maxPlacedTrees = GameController.MaxPlacedTrees;
        minForestSize = GameController.MinForestSize;
        successProcent = GameController.SuccessProcent;
        heightLimit = GameController.HeightLimit;
        minHeight = GameController.MinHeight;
    }

    // Update is called once per frame

    void Update()
    {

        if (Regenerate == true)
        {
            regen();
        }
        if (ClearList == true)
        {
            Clear();
        }
    }
    public void Clear()
    {
        ClearList = false;
        for (int i = trees.Count - 1; i >= 0; i--)
        {
            GameObject temp = trees[i];
            trees.RemoveAt(i);
            DestroyImmediate(temp);
        }
    }
    public void HideTrees()
    {
        if (TreeParent != null)
            TreeParent.SetActive(false);
    }
    public void showTrees()
    {
        if (TreeParent != null)
        {
            TreeParent.SetActive(true);
        }

    }

    public void regen()
    {
        Regenerate = false;
        for (int i = trees.Count - 1; i >= 0; i--)
        {
            GameObject temp = trees[i];
            trees.RemoveAt(i);
            DestroyImmediate(temp);
        }
        Random.InitState(2);


    }

    public bool checkCollision(Vector3 start, int iteration)
    {
        if (iteration <= 10)
        {

            RaycastHit hit;
            Vector2 r = Random.insideUnitCircle.normalized * 5f;
            Vector3 random = new Vector3(r.x, 0, r.y);
            Ray collision = new Ray(start + Vector3.up, random);
            if (Physics.Raycast(collision, out hit, 5f))
            {
                if (hit.collider.CompareTag("tree"))
                {
                    return checkCollision(start, iteration + 1);
                }
            }
        }

        return false;
    }
    public void chunkplaceTree(Vector3 StartPos, Chunk c)
    {
        if (trees.Count < 1)
        {
            Random.InitState(SyncableStorage.main.WorldSeedInt + c.Position.X + c.Position.Z);
            chunkPlaceTree(StartPos, c);
        }

    }
    public void chunkPlaceTree(Vector3 ChunkMiddle, Chunk c)
    {
        treePositions = PoissonDiscSampling.GeneratePoints(8f, new Vector2(World.chunkSize, World.chunkSize), 2, maxPlacedTrees);
        for (int i = 0; i < treePositions.Count; i++)
        {
            Vector2 item = treePositions[i];
            //Debug.DrawRay(s + ChunkMiddle + new Vector3(item.x, 0, item.y), Vector3.up, Color.green, 10f);
            var newPos = (new Vector3(Mathf.Lerp(0, World.chunkSize, item.x / World.chunkSize), 0, Mathf.Lerp(0, World.chunkSize, item.y / World.chunkSize)) + ChunkMiddle);
            Ray ray = new Ray(newPos + (Vector3.up * 10), new Vector3(0, -100, 0));
            //Debug.DrawRay(newPos + (Vector3.up * 10), new Vector3(0, -100, 0), Color.red, 100f);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.collider.CompareTag("tree") && hit.point.y < heightLimit && hit.point.y > minHeight)
                {
                    GameObject newTree = Instantiate(TreePrefab, hit.point, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, Random.Range(0f, 360f), 0)), c.chunk.transform);
                    Instantiate(placeableModels[Random.Range(0, placeableModels.Count)], newTree.transform);
                    trees.Add(newTree);
                }
            }
        }
        if (trees.Count != 0)
            SFXManagerController.Instance.PlayOnObject("env_forest", trees[0]);


    }





}




public static class PoissonDiscSampling
{

    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30, int maxNumOfPoints = 300)
    {
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(sampleRegionSize / 2);
        while (spawnPoints.Count > 0)
        {
            if (points.Count > maxNumOfPoints) return points;
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;
            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);
                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }

        }

        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
