using UnityEngine;

public class Repopulate : MonoBehaviour
{
    TreePlacement pa;
    public int id;
    private void Awake()
    {
        pa = GetComponentInParent<TreePlacement>();
        if (transform.position.y < pa.heightLimit && transform.position.y > pa.minHeight)
        {
            pa.placeTree(transform.position, id + 1);
        }
        else { Destroy(gameObject); }

    }

}
