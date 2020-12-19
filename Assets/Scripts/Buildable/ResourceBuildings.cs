using System;
using UnityEngine;
using System.Collections.Generic;
using static ResourceClass;

public class ResourceBuildings : Selectable
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
            else
            {
                try
                {
                    buildings[i].SetActive(false);
                }
                catch (Exception)
                {

                }
                
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

    public override void unSelect()
    {
        throw new NotImplementedException();
    }

    public override void Select()
    {
        throw new NotImplementedException();
    }
}
