using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resources;

public class citySystem : MonoBehaviour
{
    public int cityID;

    public List<ResourceClass> resources = new List<ResourceClass>();

    public citySystem(List<ResourceClass> resources)
    {
        this.resources = resources;
    }
    public citySystem()
    {
        resources.Add(new ResourceClass(ResourceTypes.Wood, 100, 10));
        resources.Add(new ResourceClass(ResourceTypes.Stone, 100, 10));
        resources.Add(new ResourceClass(ResourceTypes.Food, 100, 10));
        resources.Add(new ResourceClass(ResourceTypes.Iron, 100, 10));
    }

    public ResourceClass GetResource(ResourceTypes resourceType)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if(resources[i].ResourceType == resourceType)
            {
                return resources[i];
            }
        }
        return null;
    }
}
