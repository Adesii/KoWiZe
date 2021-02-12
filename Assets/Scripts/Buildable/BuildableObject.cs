
using Mirror;
using UnityEngine;

public abstract class BuildableObject : NetworkBehaviour
{

    [Header("Buildable Settings")]
    [SyncVar]
    public uint OwnerID;
    [SyncVar(hook = nameof(NewOwner))]
    public NetworkIdentity playerOwner;
    [SyncVar]
    public bool isBuilding = true;
    public GameObject currentlyBuilding;
    public GameObject built;
    [SyncVar(hook = nameof(NewParent))]
    public NetworkIdentity parent;
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
    internal virtual void NewOwner(NetworkIdentity oldO,NetworkIdentity newO)
    {
        OwnerID = newO.netId;
    }

    internal virtual void NewParent(NetworkIdentity oldP,NetworkIdentity newP)
    {
        if (parent != null)
            transform.SetParent(parent.transform);
    }

    public virtual void wantsTobeBuild()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(true);
        //effectsForBuildingMode
    }
}