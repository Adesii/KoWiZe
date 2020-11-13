using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class SettingsOperator : MonoBehaviour
{
    public UniversalRenderPipelineAsset assetToModify;
    public SettingsOptions options;


    public bool ApplySettings()
    {
        foreach (var item in options.whatSettingsToUse)
        {
            if (assetToModify.GetType().GetProperty(item.SettingName) != null)
            {
                if(item.typeofSetting == SettingsOptions.settingsType.checkBox)
                assetToModify.GetType().GetProperty(item.SettingName).SetValue(assetToModify, item.isChecked);
            }
        }




        return true;
    }
}
