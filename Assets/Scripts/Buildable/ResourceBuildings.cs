using System;
using UnityEngine;
using System.Collections.Generic;
using static ResourceClass;

public class ResourceBuildings : BuildableObject
{
    public citySystem resourceCity;
    public ResourceTypes type;
    public List<GameObject> buildings;

    public ResourceClass resource;

    public float amount = 10f;

    private void Start()
    {
        GameController.Instance.onResourceTick += onResource;
        resource = resourceCity.GetResource(type);
        for (int i = 0; i < buildings.Count; i++)
        {
            if(i == (int)type)
            {
                buildings[i].SetActive(true);
            }
            else if(buildings[i] != null)
            {
                    buildings[i].SetActive(false);
                
            }
        }
    }
    private void Update()
    {
        if (isBuilding)
        {
            transform.LookAt(resourceCity.transform);
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
        transform.parent = resourceCity.transform;
    }
    public override void HasBeenBuild()
    {
        base.HasBeenBuild();
        resourceCity.AddResourceBuilding(this);
        
    }
}
