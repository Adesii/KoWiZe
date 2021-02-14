using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AORToggle : Toggle, IAORSettingsSaver
{
    public void TriggerUpdate()
    {
        onValueChanged?.Invoke(isOn);
    }
}


public interface IAORSettingsSaver
{
    public void TriggerUpdate();
}


