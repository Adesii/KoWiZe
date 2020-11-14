using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repopulate : MonoBehaviour
{
    TreePlacement pa;
    public int id;
    private void Awake()
    {
        pa = GetComponentInParent<TreePlacement>();
        pa.placeTree(transform.position, id);
    }

}
