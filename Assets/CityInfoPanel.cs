using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityInfoPanel : MonoBehaviour
{
    public List<ResourceIcon> Iconresources = new List<ResourceIcon>();
    public TextMeshProUGUI CityName;
    public citySystem ownCity;

    public GameObject ResourceInstancing;
    public Transform ResourceParent;

    private void Awake()
    {
        GameController.Instance.OnGameTick += UpdateResources;
        GameController.UIInstance.strategyModeUI.BuildPanel.GetComponent<simpleUIFader>().onFadedOut += CityInfoPanel_onFadedOut;
        
    }
    private void OnEnable()
    {
        GameController.UIInstance.strategyModeUI.city = this;
    }
    private void CityInfoPanel_onFadedOut()
    {
        ownCity = null;
    }
    public void UpdateResources()
    {
        if (Iconresources.Count > 0)
        {
            if (ownCity != null)
            {
                foreach (var item in Iconresources)
                {

                    item.ResourceCount.text = localization.GetLocalisedValue("RR_" + item.ResourceType.ToString()) + ":\n " + item.Resourceclass.currentAmount + "/" + item.Resourceclass.maxCapacity;
                }
            }
        }
        else
        {
            if (ownCity != null)
            {
                foreach (var item in ownCity.res)
                {
                    ResourceIcon ress = Instantiate(ResourceInstancing, ResourceParent).GetComponent<ResourceIcon>();
                    ress.Resource.sprite = GameController.GetResourceIcon(item.Key);
                    ress.ResourceType = item.Key;
                    ress.Resourceclass = item.Value;
                    ress.ResourceCount.text = ress.Resourceclass.ResourceType + ":\n " + ress.Resourceclass.currentAmount + "/" + ress.Resourceclass.maxCapacity;
                    CityName.text = ownCity.name;
                    Iconresources.Add(ress);
                }
            }
        }
    }
}
