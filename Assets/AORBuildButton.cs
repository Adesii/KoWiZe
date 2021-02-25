using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum typeOfBuilding
{
    resource,
    city
}

public class AORBuildButton : MonoBehaviour
{
    public int maxAmountOfThis=10;
    public int currAmount;
    public TextMeshProUGUI maxAmount;
    public List<BaseUnit.Costs> Costs;
    public List<BaseUnit.Costs> newCosts;
    public typeOfBuilding typeOfBuilding;
    public ResourceClass.ResourceTypes resource;
    public TextMeshProUGUI Costsdisplay;
    public TextMeshProUGUI NameDisplay;
    public bool ignoreCosts = false;
    BuildPanelMenu p;


    private void Start()
    {
        p = FindObjectOfType<BuildPanelMenu>();
        p.CityChange += UpdateNumbers;
        GameController.Instance.OnGameTick +=()=>UpdateNumbers(p.CityInfoPanel.ownCity);
        UpdateNumbers(p.CityInfoPanel.ownCity);
    }

    public void UpdateNumbers(citySystem newss = null)
    {
        citySystem news = newss is null ? newss : p.CityInfoPanel.ownCity;
        if (news == null) return;
        newCosts = new List<BaseUnit.Costs>(Costs);
        string s = "";
        switch (typeOfBuilding)
        {
            case typeOfBuilding.resource:
                currAmount = news.ResourceBuilding.FindAll((e) => e.resource.ResourceType == resource).Count;
                maxAmount.text = $"{(news.ResourceBuilding.FindAll((e) => e.resource.ResourceType == resource).Count)} / {maxAmountOfThis}";
                NameDisplay.text = localization.GetLocalisedValue("RR_" + resource.ToString());
                for (int i = 0; i < Costs.Count; i++)
                {
                    BaseUnit.Costs item = Costs[i];
                    newCosts[i] = new BaseUnit.Costs
                    {
                        Resource = item.Resource,
                        Cost = Mathf.Clamp(item.Cost + 5 * currAmount, 0f, 100f)
                    };
                }
                break;
            case typeOfBuilding.city:
                for (int i = 0; i < Costs.Count; i++)
                {
                    BaseUnit.Costs item = Costs[i];
                    newCosts[i] = new BaseUnit.Costs
                    {
                        Resource = item.Resource,
                        Cost = Mathf.Clamp(item.Cost + 10 * GameController.Instance.citySettings.perPlayerSettings[GameController.Instance.localPlayerID].playerCities.Count, 0f, 100f)
                    };
                }
                break;
            default:
                break;
        }
        foreach (var item in newCosts)
        {
            s += item.ToString() + "\n";
        }
        Costsdisplay.text = s;
    }

    public void TryToBuild()
    {
        if (newCosts.All((e) => p.CityInfoPanel.ownCity.res[e.Resource].canRemoveResource(e.Cost))|| ignoreCosts)
        {
            foreach (var item in newCosts)
            {
                p.CityInfoPanel.ownCity.res[item.Resource].RemoveResource(item.Cost);
            }
            switch (typeOfBuilding)
            {
                case typeOfBuilding.resource:
                    GameController.resourceBuildMode((int)resource);
                    break;
                case typeOfBuilding.city:
                    GameController.CmdcityBuildmode();
                    break;
                default:
                    break;
            }
        }
    }
}
