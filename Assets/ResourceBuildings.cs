using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Resources;

public class ResourceBuildings : MonoBehaviour
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
}
