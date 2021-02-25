using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AORProductionQueueUpdater : MonoBehaviour
{
    [SerializeField]
    private BuildPanelMenu p;

    [SerializeField]
    private Transform viewer;
    [SerializeField]
    private GameObject Queueprefab;
    [SerializeField]
    private GameObject Wrapperprefab;
    private AORQueableItem curItem;
    private citySystem currcity;
    private Image currImage;

    private void Start()
    {
        p.CityChange += CityChange;
        GameController.Instance.OnGameTick += changFill;
        CityChange(p.CityInfoPanel.ownCity);
    }
    private void changFill()
    {
        if (viewer.childCount <= 0) return;
        if (currImage == null)
        {
            var c = viewer.GetChild(0);
            currImage = c.GetChild(c.childCount-1).GetComponent<Image>();
        }
        
    }
    private void Update()
    {
        if (currcity != null && currImage != null)
            currImage.fillAmount = Mathf.Lerp(currImage.fillAmount,currcity.Creator.currBuildTime / currcity.Creator.totalBuildTime,Time.deltaTime*4f);
    }
    private void CityChange(citySystem c)
    {
        currImage = null;
        try
        {
            currcity.Creator.QueuedNewItem -= UpdateDisplay;
            currcity.Creator.Finished -= removeUnit;
        }
        catch (System.Exception)
        {
        }

        currcity = c;
        c.Creator.QueuedNewItem += UpdateDisplay;
        c.Creator.Finished += removeUnit;

        foreach (Transform item in viewer)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in c.Creator.itemQueue)
        {
            var slot = Instantiate(Queueprefab, viewer).GetComponent<AORUISlot>();
            var go = Instantiate(Wrapperprefab, slot.transform).GetComponent<Image>();
            slot.AddItem(item);
            if (item == c.Creator.currentlyBuilding)
            {
                curItem = item;
                go.fillAmount = c.Creator.currBuildTime / c.Creator.totalBuildTime;
            }
            else
                go.fillAmount = 0f;
        }

    }

    private void UpdateDisplay(AORQueableItem item)
    {
        var slot = Instantiate(Queueprefab, viewer).GetComponent<AORUISlot>();
        var go = Instantiate(Wrapperprefab, slot.transform).GetComponent<Image>();
        slot.AddItem(item);
        go.fillAmount = 0f;
        if (item == currcity.Creator.currentlyBuilding)
        {
            //currImage = go.GetComponent<Image>();
            curItem = item;
        }
    }
    private void removeUnit(AORQueableItem item)
    {
        if (viewer.childCount > 0)
            Destroy(viewer.GetChild(0).gameObject);
        currImage = null;
    }
}
