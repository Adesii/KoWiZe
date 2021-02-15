using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelMenu : MonoBehaviour
{
    public CityInfoPanel CityInfoPanel;
    public simpleUIFader BuildPanelFader;

    public GameObject ScrollContentWindow;

    public GameObject CategoriesPrefab;

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

    private void Start()
    {

        foreach (var item in UnitManagerSingleton.Instance.AllUnits)
        {
            if (item.Value.Length <= 0) continue;
            var unitCategory = Instantiate(CategoriesPrefab, ScrollContentWindow.transform).GetComponent<AORCategorySorter>();
            unitCategory.CategoryName.text = item.Key;
            foreach (var unit in item.Value)
            {
                if (selectedUnit == null)
                    selectedUnit = unit;
                unitCategory.AddUnit(unit);
            }
        }
    }
}
