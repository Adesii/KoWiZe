
using UnityEngine;

public abstract class BuildableObject : MonoBehaviour
{
    [Header("Buildable Settings")]
    public bool isBuilding = true;
    public GameObject currentlyBuilding;
    public GameObject built;
    public virtual void HasBeenBuild()
    {
        Debug.Log("built Completed");
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(false);
        if (built != null)
            built.SetActive(true);
        isBuilding = false;
        //add other stuff that should happen when built
        gameObject.GetComponent<Collider>().enabled = true;
    }
    public virtual void wantsTobeBuild()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(true);
        //effectsForBuildingMode
    }
}