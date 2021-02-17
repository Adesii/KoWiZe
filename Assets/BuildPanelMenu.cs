using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelMenu : MonoBehaviour
{
    public CityInfoPanel CityInfoPanel;
    public simpleUIFader BuildPanelFader;

    public GameObject ScrollContentWindow;

    public GameObject CategoriesPrefab;

    public AORUnitDisplayViewer viewer;

    public AORUnitRecruitmentMasterPanel arourmp;

    public delegate void OnVariableChangeDelegate(BaseUnit newSelectedUnit);
    public static event OnVariableChangeDelegate OnSelectionChange;
    private BaseUnit curUnit;
    public BaseUnit selectedUnit
    {
        get
        { return curUnit; }
        set
        {
            if (curUnit == value) return;
            curUnit = value;
            OnSelectionChange?.Invoke(curUnit);
        }
    }

    public void FadeUI()
    {
        if (BuildPanelFader == null) BuildPanelFader = GetComponent<simpleUIFader>();
        BuildPanelFader.disableObject();
    }

    public void timerCallback(float time)
    {
        Debug.Log(time);
    }
    public void ItemFinishedCallback(AORQueableItem item)
    {
        Debug.Log("BuildpanelNoticed");
        if (BaseUnit.isUnit(item))
        {
            Debug.Log("Finished new Unit. " + item.Unit_name);
            viewer.AddNewUnitToDisplay(item);
        }
    }
    public void QueueNewItem(AORQueableItem item)
    {
        item.ownerCity = CityInfoPanel.ownCity;
        CityInfoPanel.ownCity.Creator.QueueNewItem(item);
    }
    private void Awake()
    {
        CityInfoPanel.citySelectionChanged += registerNewCities;
        CityInfoPanel.citySelectionChanged += viewer.onCityChange;
    }
    private void Start()
    {
        


        foreach (var item in UnitManagerSingleton.Instance.AllUnits)
        {
            if (item.Value.Length <= 0) continue;
            var unitCategory = Instantiate(CategoriesPrefab, ScrollContentWindow.transform).GetComponent<AORCategorySorter>();
            unitCategory.CategoryName.text = item.Key;
            unitCategory.p = this;
            foreach (var unit in item.Value)
            {
                if (selectedUnit == null)
                    selectedUnit = unit;
                unitCategory.AddUnit(unit);
            }
        }
    }

    public void registerNewCities(citySystem arg1,citySystem arg2)
    {
        if(arg1 != null)
        {
            arg1.Creator.LeftForCurrentItem -= timerCallback;
            arg1.Creator.Finished -= ItemFinishedCallback;
        }
        if(arg2 != null)
        {
            arg2.Creator.LeftForCurrentItem += timerCallback;
            arg2.Creator.Finished += ItemFinishedCallback;
        }
            
    }
}
