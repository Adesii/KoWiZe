using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_City_Hover_prefab_Store : MonoBehaviour
{
    public List<ResourceIcon> Iconresources = new List<ResourceIcon>();
    public TextMeshProUGUI CityName;
    public GameObject ResourceInstancing;
    public Transform ResourceParent;

    public citySystem ownCity;
    public Canvas sortingStuff;

    public float UIscale;

    private void Awake()
    {
        GameController.Instance.OnGameTick += UpdateResources;

    }
    private void OnEnable()
    {
        transform.DOScale(UIscale, 0.2f);
    }
    public void disableObject()
    {
        transform.DOScale(0, 0.2f).OnComplete(() => { gameObject.SetActive(false); });
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
                    CityName.text = gameObject.name;
                    Iconresources.Add(ress);
                }
            }
        }
    }
}
