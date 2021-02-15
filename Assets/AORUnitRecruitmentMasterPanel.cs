using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AORUnitRecruitmentMasterPanel : MonoBehaviour
{
    [SerializeField]
    Image UnitPicture;
    [SerializeField]
    TextMeshProUGUI UnitName;
    [SerializeField]
    TextMeshProUGUI UnitInfo;
    [SerializeField]
    TextMeshProUGUI UnitStrenghts;

    private void Awake()
    {
        BuildPanelMenu.OnSelectionChange += onSelectedUnit;
    }
    public void onSelectedUnit(BaseUnit unit)
    {
        if (unit.UnitIcon != null)
            UnitPicture.sprite = unit.UnitIcon.sprite;
        UnitName.text = unit.Unit_name;
        UnitInfo.text = unit.description;
        UnitStrenghts.text = "None lol";
    }
}
