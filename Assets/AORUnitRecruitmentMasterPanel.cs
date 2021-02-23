using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }
    private void Start()
    {
        BuildPanelMenu.OnSelectionChange += onSelectedUnit;

    }
    public void ProduceUnit()
    {
        currUnit.QueueType = 1;
        if(currUnit.costs.All((e) => panel.CityInfoPanel.ownCity.res[e.Resource].canRemoveResource(e.Cost)))
        {
            foreach (var item in currUnit.costs)
            {
                panel.CityInfoPanel.ownCity.res[item.Resource].RemoveResource(item.Cost);
            }
            panel.QueueNewItem(currUnit);
        }

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
