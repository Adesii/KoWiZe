using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AORCategorySorter : MonoBehaviour
{
    public TextMeshProUGUI CategoryName;
    [SerializeField]
    public GameObject Units;
    [SerializeField]
    GameObject UnitPrefab;

    Dictionary<string, BaseUnit> unitDic = new Dictionary<string, BaseUnit>();

    public BuildPanelMenu p;

    public LayoutGroup lg;

    public void AddUnit(BaseUnit unit)
    {
        if (unitDic.ContainsKey(unit.Unit_name)) return;
        var go =Instantiate(UnitPrefab,Units.transform).GetComponent<AORCategoryUnit>();
        go.UnitName.text = unit.Unit_name;
        go.b = p;
        go.ownUnit = unit;
        if(unit.UnitIcon != null)
        go.UnitPicture.sprite = unit.UnitIcon;
        unitDic.Add(unit.Unit_name, unit);
    }

    public void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
