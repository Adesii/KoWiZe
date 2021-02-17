using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AORUISlot : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI amount;
    [SerializeField]
    private Image picture;
    [SerializeField]
    private TextMeshProUGUI slotName;

    public string UnitTypeName = "Base";

    private List<AORQueableItem> ItemSlot = new List<AORQueableItem>();
    public void AddItem(AORQueableItem item)
    {
        if(ItemSlot.Count == 0)
        {
            UnitTypeName = item.Unit_name;
            slotName.text = UnitTypeName;
            if (item.UnitIcon != null)
            picture.sprite = item.UnitIcon;
        }
        ItemSlot.Add(item);
        amount.text = ItemSlot.Count.ToString();
    }
}
