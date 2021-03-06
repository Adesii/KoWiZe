﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

    public List<ResourceClass> costs;

    public float amount = 10f;

    internal override void NewParent(NetworkIdentity oldP, NetworkIdentity newP)
    {
        base.NewParent(oldP, newP);
        if (oldP != null)
            oldP.GetComponent<citySystem>().RemoveResourceBuilding(this);
        Debug.Log($"Hook Called on {netIdentity.name}");
        resourceCity = newP.GetComponent<citySystem>();
        CmdInitOnAll();
        resourceCity.AddResourceBuilding(this);
    }
    private void Update()
    {
        if (isBuilding)
        {
            transform.LookAt(resourceCity.transform);
        }
    }
    [Command]
    public void CmdInitOnAll()
    {
        Rpcinit();
    }
    [ClientRpc]
    public void Rpcinit()
    {
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
            resource.AddResource(GameController.Instance.localSettings.GainAmount.Find((e) => e.Resource == resource.ResourceType).amount);
    }

    public override void wantsTobeBuild()
    {
        base.wantsTobeBuild();
    }
    public override bool HasBeenBuild()
    {
        if (!base.HasBeenBuild()) return false;
        CmdInitOnAll();
        GameController.Instance.onResourceTick += onResource;
        resourceCity.ShowResources();

        return true;
    }
}
