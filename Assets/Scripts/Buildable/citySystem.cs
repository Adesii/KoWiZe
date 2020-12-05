using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ResourceClass;

public class citySystem : BuildableObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int cityID;
    public int cityTier = 0;
    public List<GameObject> buildings;
    public GameObject SelectionRing;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;

    public GameObject built;
    public GameObject currentlyBuilding;
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
        Debug.Log("built Completed");
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(false);
        if (built != null)
            built.SetActive(true);
        isBuilding = false;
        //add other stuff that should happen when built
        gameObject.GetComponent<Collider>().enabled = true;

        gm = UI_City_Hover.addNewCity(this);
    }

    public override void wantsTobeBuild()
    {
        Debug.Log("building Object : " + cityID + ": ID ; " + gameObject.name + " : name;");
        gameObject.GetComponent<Collider>().enabled = false;
        if (currentlyBuilding != null)
            currentlyBuilding.SetActive(true);
        //effectsForBuildingMode
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && !isBuilding)
        {
            print("You Hovered: " + gameObject.name);
            if (gm != null) gm.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && !isBuilding)
        {
            print("You Exited: " + gameObject.name);
            if (gm != null) gm.disableObject();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuilding)
            GameController.Instance.localSettings.localPlayer.AddToSelection(this);
    }

    public override void unSelect()
    {
        isSelected = false;
        if (gm != null)
        {
            gm.disableObject();
            SelectionRing.SetActive(false);
        }
    }

    public override void Select()
    {
        isSelected = true;
        if (gm != null)
        {
            gm.gameObject.SetActive(true);
            SelectionRing.SetActive(true);
        }
    }
    [System.Serializable]
    public class ResResClassDictionary : SerializableDictionary<ResourceTypes, ResourceClass> { }
}
