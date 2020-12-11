using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ResourceClass;
using Mirror;

public class citySystem : Selectable
{
    public int cityID;
    public int cityTier = 0;
    public List<GameObject> buildings;
    public GameObject SelectionRing;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;
    public List<ResourceBuildings> ResourceBuilding= new List<ResourceBuildings>();

    
    
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



    private void Update()
    {
        if (isSelected)
        {
            foreach (var item in ResourceBuilding)
            {
                Debug.DrawLine(transform.position, item.transform.position, new Color(0, 0, 255), 1f, false);
            }
            Debug.Log(ResourceBuilding.Count);
        }
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
        gm = UI_City_Hover.addNewCity(this);
    }

    public override void PointerClicked()
    {
        base.PointerClicked();

    }

    public override void PointerEntered()
    {
        base.PointerEntered();
        if (!isSelected && !isBuilding)
        {
            print("You Hovered: " + gameObject.name);
            if (gm != null) gm.gameObject.SetActive(true);
        }
    }
    public override void PointerExited()
    {
        base.PointerExited();
        if (!isSelected && !isBuilding)
        {
            print("You Exited: " + gameObject.name);
            if (gm != null) gm.disableObject();
        }
    }


    public override void unSelect()
    {
        isSelected = false;
        if (gm != null)
        {
            gm.disableObject();
            SelectionRing.SetActive(false);
        }
        GameController.UIInstance.strategyModeUI.BuildPanel.GetComponent<simpleUIFader>().disableObject();

    }

    public override void Select()
    {
        isSelected = true;
        if (gm != null)
        {
            gm.gameObject.SetActive(true);
            SelectionRing.SetActive(true);
        }
        GameController.UIInstance.strategyModeUI.BuildPanel.SetActive(true);
    }











    [System.Serializable]
    public class ResResClassDictionary : SerializableDictionary<ResourceTypes, ResourceClass> { }
}
