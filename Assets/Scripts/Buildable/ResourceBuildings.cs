using System;
using UnityEngine;
using System.Collections.Generic;
using static ResourceClass;
using Mirror;

public class ResourceBuildings : BuildableObject
{
    [SyncVar]
    public int OwnerCityID;
    public citySystem resourceCity;
    [SyncVar]
    public ResourceTypes type;
    public List<GameObject> buildings;

    public ResourceClass resource;

    public float amount = 10f;

    internal override void NewParent(NetworkIdentity oldP, NetworkIdentity newP)
    {
        base.NewParent(oldP,newP);
        Debug.Log($"Hook Called on {netIdentity.name}");
        resourceCity = newP.GetComponent<citySystem>();
        init();
        resourceCity.AddResourceBuilding(this);
    }
    private void Update()
    {
        if (isBuilding)
        {
            transform.LookAt(resourceCity.transform);
        }
    }
    public void init()
    {
        GameController.Instance.onResourceTick += onResource;
        resource = resourceCity.GetResource(type);
        for (int i = 0; i < buildings.Count; i++)
        {
            if (i == (int)type)
            {
                buildings[i].SetActive(true);
            }
            else if (buildings[i] != null)
            {
                buildings[i].SetActive(false);

            }
        }
    }
    private void onResource()
    {
        if (resource != null)
            resource.AddResource(amount);
    }

    public override void wantsTobeBuild()
    {
        base.wantsTobeBuild();
    }
    public override void HasBeenBuild()
    {
        base.HasBeenBuild();
        init();
        resourceCity.AddResourceBuilding(this);

    }
}
