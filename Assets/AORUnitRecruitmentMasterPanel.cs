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
    GameObject UnitStrenghts;
    [SerializeField]
    GameObject UnitAttrPrefab;
    [SerializeField]
    BuildPanelMenu panel;

    BaseUnit currUnit;

    private void Awake()
    {
        BuildPanelMenu.OnSelectionChange += onSelectedUnit;
    }

    public void ProduceUnit()
    {
        Debug.Log("presseButton");
        currUnit.QueueType = 1;
        panel.QueueNewItem(currUnit);
    }

    public void onSelectedUnit(BaseUnit unit)
    {

        currUnit = unit;
        //if (unit.UnitIcon != null)
        //    UnitPicture.sprite = unit.UnitIcon.sprite;
        UnitName.text = unit.Unit_name;
        UnitInfo.text = unit.description;

        foreach (Transform child in UnitStrenghts.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in unit.GetStats())
        {
            if (item.Value == null) continue;
            var go = Instantiate(UnitAttrPrefab, UnitStrenghts.transform).GetComponent<AORUnitAttrShow>();
            switch (item.Key)
            {
                case "Unit_Costs":
                    var gs = "";
                    foreach (var g in item.Value as List<BaseUnit.Costs>)
                    {
                        gs += g.ToString() + "\n";
                    }
                    go.SetAttr(item.Key, gs);
                    break;
                case "Unit_Strenghts":
                    var s = "";
                    foreach (var ss in item.Value as List<BaseUnit.UnitStrenghts>)
                    {
                        s += ss.ToString()+ "\n";
                    }
                    go.SetAttr(item.Key, s);
                    break;
                default:
                    go.SetAttr(item.Key, item.Value.ToString());
                    break;
            }
        }
    }
}
