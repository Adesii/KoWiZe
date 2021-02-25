using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildPanelMenu : MonoBehaviour
{
    public CityInfoPanel CityInfoPanel;
    public simpleUIFader BuildPanelFader;

    public GameObject ScrollContentWindow;

    public GameObject CategoriesPrefab;

    public AORUnitDisplayViewer viewer;

    public AORUnitRecruitmentMasterPanel arourmp;
    public Action<citySystem> CityChange;

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
        TechTreeManager.Instance.NewTechUnlocked += UpdateViewSelection;
    }
    private void Start()
    {
        UpdateViewSelection();
    }

    private Dictionary<string, AORCategorySorter> CreatedCategories = new Dictionary<string, AORCategorySorter>();

    private void UpdateViewSelection()
    {
        foreach (var item in GameController.Instance.localSettings.LocalPlayerUnlockedUnits)
        {
            if (CreatedCategories.TryGetValue(item.Categorie, out AORCategorySorter val))
            {
                val.AddUnit(item);
                val.UpdateLayout();
            }
            else
            {

                var v = Instantiate(CategoriesPrefab, ScrollContentWindow.transform).GetComponent<AORCategorySorter>();
                v.CategoryName.text = localization.GetLocalisedValue(item.Categorie);
                if (string.IsNullOrWhiteSpace(v.CategoryName.text))
                    v.CategoryName.text = item.Categorie;
                v.p = this;
                v.AddUnit(item);
                CreatedCategories.Add(item.Categorie, v);
                v.UpdateLayout();

            }
            if (selectedUnit == null)
                selectedUnit = item;
        }
        arourmp.onSelectedUnit(selectedUnit);
    }


    public void registerNewCities(citySystem arg1, citySystem arg2)
    {
        if (arg1 != null)
        {
            arg1.Creator.LeftForCurrentItem -= timerCallback;
            arg1.Creator.Finished -= ItemFinishedCallback;
        }
        if (arg2 != null)
        {
            arg2.Creator.LeftForCurrentItem += timerCallback;
            arg2.Creator.Finished += ItemFinishedCallback;

            CityChange?.Invoke(arg2);
        }

    }
}
