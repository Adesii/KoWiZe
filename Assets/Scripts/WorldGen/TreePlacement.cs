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

    GameObject TreeParent;


    // Start is called before the first frame update
    void Start()
    {
        TreePrefab = GameController.TreePrefab;
        placeableModels = GameController.PlaceableModels;
        maxPlacedTrees = GameController.MaxPlacedTrees;
        minForestSize = GameController.MinForestSize;
        successProcent = GameController.SuccessProcent;
        heightLimit = GameController.HeightLimit;
        minHeight = GameController.MinHeight;
        TreeParent = new GameObject();
        TreeParent.transform.parent = transform;
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
        placeTree(transform.position, 0);


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

    public void placeTree(Vector3 StartPos, int id)
    {
        if (trees.Count <= maxPlacedTrees)
            co = StartCoroutine(PlaceTree(StartPos, id));
    }
    public void chunkplaceTree(Vector3 StartPos, int id)
    {
        if (trees.Count <= maxPlacedTrees)
        {
            co = StartCoroutine(chunkPlaceTree(StartPos, id));
        }
            
    }
    public IEnumerator chunkPlaceTree(Vector3 startSpot, int id)
    {
        yield return new WaitForSeconds(0.25f);
        if (maxPlacedTrees <= trees.Count) yield break;

        if (id > maxForestSize) yield break;
        Vector2 r = Vector2.zero;
        if (trees.Count < 1) r = Random.insideUnitCircle.normalized * Random.Range(1f, 15f);
        else r = ((absoluteStartSpot - trees[trees.Count - 1].transform.position).normalized + Random.insideUnitSphere) * Random.Range(1f, 15f) * id;
        Vector3 random = new Vector3(r.x, 0, r.y);
        RaycastHit hit;
        Ray ray = new Ray(startSpot + (Vector3.up * 10), new Vector3(0, -1, 0));

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.CompareTag("tree") && hit.point.y < heightLimit && hit.point.y > minHeight && !hit.collider.CompareTag("enviroment"))
            {
                if (id == 0) absoluteStartSpot = hit.point;
                GameObject newTree = Instantiate(TreePrefab, hit.point, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, Random.Range(0f, 360f), 0)), TreeParent.transform);
                newTree.GetComponent<Repopulate>().id = id + 1;
                Instantiate(placeableModels[Random.Range(0, placeableModels.Count)], newTree.transform);
                if (id == 0) SFXManagerController.Instance.PlayOnObject("env_forest", newTree);
                trees.Add(newTree);
            }
            else if (id < minForestSize)
            {
                if (trees.Count > 0)
                    placeTree(trees[trees.Count - 1].transform.position + random, id - 1);
            }
        }
        if (id < minForestSize)
        {
            placeTree(startSpot + random, id + 1);
            yield break;
        }
        if (Random.value <= successProcent)
        {
            placeTree(startSpot + random, id + 1);
            placeTree(startSpot + random, id + 1);
        }
    }




    public IEnumerator PlaceTree(Vector3 startSpot, int id)
    {
        yield return new WaitForSeconds(0.125f);
        if (maxPlacedTrees <= trees.Count) yield break;

        if (id > maxForestSize) yield break;
        Vector2 r = Vector2.zero;
        if (trees.Count < 1) r = Random.insideUnitCircle.normalized * Random.Range(1f, 15f);
        else r = ((absoluteStartSpot - trees[trees.Count-1].transform.position).normalized+Random.insideUnitSphere) * Random.Range(1f, 15f)*id;
        Vector3 random = new Vector3(r.x, 0, r.y);
        RaycastHit hit;
        Ray ray = new Ray(startSpot + (Vector3.up * 10), new Vector3(0, -1, 0));

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.CompareTag("tree") && hit.point.y < heightLimit && hit.point.y > minHeight && !hit.collider.CompareTag("enviroment"))
            {
                if (id == 0) absoluteStartSpot = hit.point;
                GameObject newTree = Instantiate(TreePrefab, hit.point, Quaternion.Euler(transform.rotation.eulerAngles+new Vector3(0,Random.Range(0f,360f),0)), TreeParent.transform);
                newTree.GetComponent<Repopulate>().id = id + 1;
                Instantiate(placeableModels[Random.Range(0, placeableModels.Count)], newTree.transform);
                trees.Add(newTree);
            }
            else if (id < minForestSize)
            {
                if (trees.Count > 0)
                    placeTree(trees[trees.Count - 1].transform.position + random, id - 1);
            }
        }
        if (id < minForestSize)
        {
            placeTree(startSpot + random, id + 1);
            yield break;
        }
        if (Random.value <= successProcent)
        {
            placeTree(startSpot + random, id + 1);
            placeTree(startSpot + random, id + 1);
        }
    }
}
