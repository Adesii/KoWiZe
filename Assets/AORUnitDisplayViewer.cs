using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AORUnitDisplayViewer : MonoBehaviour
{
    [SerializeField]
    private GameObject SlotPrefab;
    [SerializeField]
    private Transform contentView;
    
    public Dictionary<string,AORUISlot> UnitDisplay = new Dictionary<string, AORUISlot>();

    public void AddNewUnitToDisplay(AORQueableItem unit)
    {
        if (UnitDisplay.TryGetValue(unit.Unit_name,out AORUISlot slot))
        {
            slot.AddItem(unit);
        }
        else
        {
            var aoSlot = Instantiate(SlotPrefab, contentView).GetComponent<AORUISlot>();
            UnitDisplay.Add(unit.Unit_name, aoSlot);
            aoSlot.AddItem(unit);
        }
    }
}