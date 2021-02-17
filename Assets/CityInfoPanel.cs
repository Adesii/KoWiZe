using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityInfoPanel : MonoBehaviour
{
    public Action<citySystem, citySystem> citySelectionChanged;

    public List<ResourceIcon> Iconresources = new List<ResourceIcon>();
    public TextMeshProUGUI CityName;
    private citySystem ownCityy;
    public citySystem ownCity
    {
        get
        {
            return ownCityy;
        }
        set
        {

            citySelectionChanged.Invoke(ownCityy, value);
            ownCityy = value;
        }
    }

    public GameObject ResourceInstancing;
    public Transform ResourceParent;

    private void Awake()
    {
        GameController.Instance.OnGameTick += UpdateResources;
        GameController.UIInstance.strategyModeUI.BuildPanel.GetComponent<simpleUIFader>().onFadedOut += CityInfoPanel_onFadedOut;
    }
    private void OnEnable()
    {
        //GameController.UIInstance.strategyModeUI.BuildPanel = gameObject;
    }
    private void CityInfoPanel_onFadedOut()
    {
        //ownCity = null;
    }
    public void UpdateResources()
    {
        if (ownCity != null)
        {
            CityName.text = ownCity.name;
            if (Iconresources.Count > 0)
            {
                
                foreach (var item in Iconresources)
                {
                    item.Resourceclass = ownCity.GetResource(item.ResourceType);
                    item.ResourceCount.text = localization.GetLocalisedValue("RR_" + item.ResourceType.ToString()) + ":\n " + item.Resourceclass.currentAmount + "/" + item.Resourceclass.maxCapacity;
                }
            }
            else
            {

                foreach (var item in ownCity.res)
                {
                    ResourceIcon ress = Instantiate(ResourceInstancing, ResourceParent).GetComponent<ResourceIcon>();
                    ress.Resource.sprite = GameController.GetResourceIcon(item.Key);
                    ress.ResourceType = item.Key;
                    ress.Resourceclass = item.Value;
                    ress.ResourceCount.text = ress.Resourceclass.ResourceType + ":\n " + ress.Resourceclass.currentAmount + "/" + ress.Resourceclass.maxCapacity;
                    Iconresources.Add(ress);
                }

            }
        }
    }
}
