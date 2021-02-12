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

    private void Start()
    {
        if (resourceCity == null)
        {
            resourceCity = GameController.TryGetCityFromIDs(OwnerID, OwnerCityID);
            if (resourceCity != null) init();
        }

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
        parent = resourceCity.netIdentity;
    }
    public override void HasBeenBuild()
    {
        base.HasBeenBuild();
        init();
        resourceCity.AddResourceBuilding(this);

    }
}
