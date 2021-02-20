using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AORSlide : Slider,IAORSettingsSaver
{
    public void TriggerUpdate()
    {
        onValueChanged?.Invoke(value);
        
    }
}
