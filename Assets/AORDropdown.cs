using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AORDropdown : TMPro.TMP_Dropdown,IAORSettingsSaver
{
    public void TriggerUpdate()
    {
        onValueChanged?.Invoke(value);
    }
    protected override void Start()
    {
        foreach (var item in GetComponentsInChildren<TextLocalisationUI>())
        {
            item.changeLangue();
        }
    }
}
