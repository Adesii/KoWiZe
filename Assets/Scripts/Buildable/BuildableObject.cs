
using Mirror;
using UnityEngine;

public abstract class BuildableObject : NetworkBehaviour
{

    [Header("Buildable Settings")]
    [SyncVar]
    public int OwnerID = -1;
    [SyncVar]
    public bool isBuilding = true;
    public GameObject currentlyBuilding;
    public GameObject built;
    [SyncVar]
    public NetworkIdentity parent;
    public virtual void HasBeenBuild()
    {
        Debug.Log("built Completed");
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(false);
        if (built != null)
            built.SetActive(true);
        isBuilding = false;
        if (parent != null)
            transform.SetParent(parent.transform);
        //add other stuff that should happen when built
        gameObject.GetComponent<Collider>().enabled = true;
        if (OwnerID == -1)
        {
            OwnerID = (int)GameController.Instance.localSettings.localPlayer.netId;
            CmdSendHasBeenBuildRPC();
        }


    }
    [Command(ignoreAuthority = true)]
    public void CmdSendHasBeenBuildRPC()
    {
        GameController.RpctriggerHasBeenBuilton(netIdentity);
    }
    public virtual void wantsTobeBuild()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(true);
        //effectsForBuildingMode
    }
}