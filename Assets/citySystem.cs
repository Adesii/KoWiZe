using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceClass;

public class citySystem : MonoBehaviour
{
    public int cityID;
    public int cityTier = 0;
    public List<GameObject> buildings;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;

    
    public SerializableDictionary<ResourceTypes, ResourceClass> resources = new SerializableDictionary<ResourceTypes, ResourceClass>();


    public citySystem(SerializableDictionary<ResourceTypes, ResourceClass> resources)
    {
        this.resources = resources;
    }
    public citySystem()
    {
        resources.Add(ResourceTypes.Wood,new ResourceClass(ResourceTypes.Wood, 100, 10));
        resources.Add(ResourceTypes.Stone,new ResourceClass(ResourceTypes.Stone, 100, 10));
        resources.Add(ResourceTypes.Food,new ResourceClass(ResourceTypes.Food, 100, 10));
        resources.Add(ResourceTypes.Iron,new ResourceClass(ResourceTypes.Iron, 100, 10));
    }

    public ResourceClass GetResource(ResourceTypes resourceType)
    {
        if(resources[resourceType] != null)
        {
            return resources[resourceType];
        }
        return null;
    }
}
