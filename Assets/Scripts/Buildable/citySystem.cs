using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ResourceClass;
using Mirror;

public class citySystem : Selectable
{
    [Header("CitySettings")]
    public int cityID;
    public int cityTier = 0;
    public List<GameObject> buildings;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;
    public List<ResourceBuildings> ResourceBuilding = new List<ResourceBuildings>();



    public Transform hoverCityPosition;

    public UI_City_Hover_prefab_Store gm;

    [SerializeField]
    ResResClassDictionary resource = new ResResClassDictionary();
    public IDictionary<ResourceTypes, ResourceClass> res
    {
        get { return resource; }
        set { resource.CopyFrom(value); }
    }

    public citySystem(IDictionary<ResourceTypes, ResourceClass> resources)
    {
        this.res = (ResResClassDictionary)resources;
    }
    public citySystem()
    {
        res.Add(ResourceTypes.Wood, new ResourceClass(ResourceTypes.Wood, 100, 10));
        res.Add(ResourceTypes.Stone, new ResourceClass(ResourceTypes.Stone, 100, 10));
        res.Add(ResourceTypes.Food, new ResourceClass(ResourceTypes.Food, 100, 10));
        res.Add(ResourceTypes.Iron, new ResourceClass(ResourceTypes.Iron, 100, 10));
    }
    public ResourceClass GetResource(ResourceTypes resourceType)
    {
        print(resourceType);
        if (res.TryGetValue(resourceType, out ResourceClass re))
        {
            return re;
        }
        return null;
    }
    public override void HasBeenBuild()
    {
        base.HasBeenBuild();
        isSelected = false;
        isBuilding = false;
        gm = UI_City_Hover.addNewCity(this);
    }

    public override void PointerClicked()
    {
        base.PointerClicked();

    }

    public override void PointerEntered()
    {
        base.PointerEntered();
        if (gm != null && !isSelected) gm.gameObject.SetActive(true);
    }
    public override void PointerExited()
    {
        base.PointerExited();
        if (gm != null && !isSelected) gm.disableObject();
    }


    public override void unSelect()
    {
        base.unSelect();
        if (gm != null)
        {
            gm.disableObject();
        }
        GameController.UIInstance.strategyModeUI.BuildPanel.GetComponent<simpleUIFader>().disableObject();

    }

    public override void Select()
    {
        base.Select();
        if (gm != null)
        {
            gm.gameObject.SetActive(true);
        }
        GameController.UIInstance.strategyModeUI.BuildPanel.SetActive(true);
    }











    [System.Serializable]
    public class ResResClassDictionary : SerializableDictionary<ResourceTypes, ResourceClass> { }
}
