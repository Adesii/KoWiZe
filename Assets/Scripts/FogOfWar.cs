using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{

    public float FogRadius = 10f;
    public Mesh FogMesh;
    public Material FogMaterial;
    public int layer;
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject fogOfWar = new GameObject();
        fogOfWar.transform.parent = transform;
        fogOfWar.AddComponent<MeshFilter>().sharedMesh = FogMesh;
        fogOfWar.AddComponent<MeshRenderer>().sharedMaterial = FogMaterial;
        fogOfWar.layer = layer;
        fogOfWar.transform.localScale = new Vector3(FogRadius, FogRadius, FogRadius);
    }
}
