using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AORCategorySorter : MonoBehaviour
{
    public TextMeshProUGUI CategoryName;
    [SerializeField]
    GameObject Units;
    [SerializeField]
    GameObject UnitPrefab;

    public BuildPanelMenu p;

    public void AddUnit(BaseUnit unit)
    {
        var go =Instantiate(UnitPrefab,Units.transform).GetComponent<AORCategoryUnit>();
        go.UnitName.text = unit.Unit_name;
        go.b = p;
        go.ownUnit = unit;
        if(unit.UnitIcon != null)
        go.UnitPicture.sprite = unit.UnitIcon.sprite;
    }
}
