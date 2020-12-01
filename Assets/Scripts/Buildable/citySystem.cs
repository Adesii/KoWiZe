using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static ResourceClass;

public class citySystem : BuildableObject
{
    public int cityID;
    public int cityTier = 0;
    public List<GameObject> buildings;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;

    public GameObject built;
    public GameObject currentlyBuilding;

    
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

    public override void HasBeenBuild()
    {
        Debug.Log("built Completed");
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(false);
        if (built != null)
            built.SetActive(true);
        isBuilding = false;
        //add other stuff that should happen when built
    }

    public override void wantsTobeBuild()
    {
        Debug.Log("building Object : " + cityID + ": ID ; " + gameObject.name + " : name;");
        if(currentlyBuilding != null)
        currentlyBuilding.SetActive(true);
        //effectsForBuildingMode
    }
}
