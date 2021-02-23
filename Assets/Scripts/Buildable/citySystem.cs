using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ResourceClass;
using Mirror;
using UnityEngine.VFX;

public class citySystem : Selectable
{
    [Header("CitySettings")]
    [SyncVar]
    public int cityID;

    public int cityTier = 0;
    public int maxAmountOfBuildings = 10;
    public List<GameObject> buildings;

    public float cityPopulation;
    public List<float> cityPopulationLimitPerTier;
    private List<ResourceBuildings> ResourceBuilding = new List<ResourceBuildings>();

    public BuildPanelMenu buildPanel;

    public Transform hoverCityPosition;

    public UI_City_Hover_prefab_Store gm;
    public List<VisualEffect> vsfL;
    public GameObject arrowPrefab;
    public Gradient colorGradient;
    public AORBuildCreator Creator;

    public List<AORQueableItem> UnitInventory = new List<AORQueableItem>();


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
        //print(resourceType);
        if (res.TryGetValue(resourceType, out ResourceClass re))
        {
            return re;
        }
        return null;
    }
    public override void HasBeenBuild()
    {
        base.HasBeenBuild();
        name = "City: " + getCityName();
        isSelected = false;
        isBuilding = false;
        gm = UI_City_Hover.addNewCity(this);
        buildPanel = GameController.UIInstance.strategyModeUI.BuildPanelScript;

        cityID = GameController.Instance.citySettings.perPlayerSettings[GameController.Instance.localPlayerID].playerCities.Count - 1;

        Creator = new AORBuildCreator();
        Creator.Finished += onFinishedCallback;
    }

    private void onFinishedCallback(AORQueableItem item)
    {
        Debug.Log("Finished");
        if (BaseUnit.isUnit(item))
        {
            UnitInventory.Add(item);
        }
    }

    private string getCityName()
    {
        List<string> liss = new List<string>() { "Rosevelt", "Bobs Burger Palace", "SomethingNice", "Not a Real City", "Aachen", "Koeln" };
        return liss[Random.Range(0, liss.Count)];
    }
    public override void PointerClicked()
    {
        base.PointerClicked();

    }

    public override void PointerEntered()
    {
        base.PointerEntered();
        //if (gm != null && !isSelected) gm.gameObject.SetActive(true);
    }
    public override void PointerExited()
    {
        base.PointerExited();
        //if (gm != null && !isSelected) gm.disableObject();
    }


    public override void unSelect()
    {
        base.unSelect();
        //if (gm != null) gm.disableObject();
        Debug.Log(GameController.Instance.localSettings.localPlayer.Currently_Selected.Count);
        if (GameController.Instance.localSettings.localPlayer.Currently_Selected.Count <= 0 || GameController.Instance.localSettings.localPlayer.Currently_Selected[0].GetType() != typeof(citySystem))
            GameController.UIInstance.strategyModeUI.BuildPanelScript.FadeUI();
        hideResources();
    }

    public override void Select()
    {
        base.Select();
        /*if (gm != null)
        {
            gm.gameObject.SetActive(true);
            gm.ownCity = this;
        }
        */
        buildPanel.gameObject.SetActive(true);
        buildPanel.CityInfoPanel.ownCity = this;
        buildPanel.CityInfoPanel.UpdateResources();
        ShowResources();
    }

    private void hideResources()
    {
        foreach (var item in vsfL)
        {
            item.Stop();

        }
    }

    public void ShowResources()
    {
        int index = 0;
        foreach (var item in vsfL)
        {

            item.SetVector3("start", transform.position);
            item.SetVector3("end", ResourceBuilding[index].transform.position);
            //item.SetGradient("colorGradient", colorGradient);
            item.SetFloat("resourceColor", Mathf.Clamp(((float)ResourceBuilding[index].type), 0f, 100f));
            item.Play();
            index++;
        }
    }
    public bool AddResourceBuilding(ResourceBuildings building)
    {
        if (ResourceBuilding.Count >= maxAmountOfBuildings)
        {
            return false;
        }
        building.OwnerCityID = cityID;
        ResourceBuilding.Add(building);

        vsfL.Add(Instantiate(arrowPrefab, transform).GetComponent<VisualEffect>());
        if (!isSelected)
        {
            vsfL[vsfL.Count - 1].Stop();

        }
        return true;
    }
    public bool RemoveResourceBuilding(ResourceBuildings building)
    {
        var ind = ResourceBuilding.IndexOf(building);
        Destroy(vsfL[ind].gameObject);
        vsfL.RemoveAt(ind);
        ResourceBuilding.Remove(building);
        return true;
    }

    internal override void NewOwner(NetworkIdentity oldO, NetworkIdentity newO)
    {
        base.NewOwner(oldO, newO);
        if(oldO != null)
        {
            var pps = GameController.CitySettings.perPlayerSettings[GameController.GetPlayerIndexbyNetID(oldO.netId)];
            if (pps.playerCities.Contains(this))
                pps.playerCities.Remove(this);
        }
        GameController.CitySettings.perPlayerSettings[GameController.GetPlayerIndexbyNetID(OwnerID)].playerCities.Add(this);
    }

    public void QueueNewUnit(BaseUnit unit)
    {

    }








    [System.Serializable]
    public class ResResClassDictionary : SerializableDictionary<ResourceTypes, ResourceClass> { }
}
