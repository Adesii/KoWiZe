using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ResourceClass;

public class ResourceBuildings : BuildableObject
{
    public citySystem resourceCity;
    public ResourceTypes type;

    public ResourceClass resource;

    public float amount = 10f;

    private void Start()
    {
        GameController.Instance.onResourceTick += onResource;
        resource = resourceCity.GetResource(type);
    }

    private void onResource()
    {
        resource.AddResource(amount);
    }

    public override void HasBeenBuild()
    {
        throw new NotImplementedException();
    }

    public override void wantsTobeBuild()
    {
        throw new NotImplementedException();
    }
}
