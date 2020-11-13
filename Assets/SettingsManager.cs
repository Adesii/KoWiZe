using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SettingsOptions.settingSetting;

public class SettingsManager : MonoBehaviour
{
    public SettingsOptions options;
    public List<Isettings> settings;

    private void OnEnable()
    {

    }

    private void fillListOfSettings()
    {
        if (settings.Count < 1)
        {
            foreach (var item in options.whatSettingsToUse)
            {
                switch (item.typeofSetting)
                {
                    case SettingsOptions.settingsType.dropDown:
                        
                        settings.Add(new settingValues<List<dropdownSettings>>(item.SettingName, item.dropDownOptions));

                        break;
                    case SettingsOptions.settingsType.slider:
                        settings.Add(new settingValues<int>(item.SettingName, item.value));

                        break;
                    case SettingsOptions.settingsType.checkBox:
                        settings.Add(new settingValues<bool>(item.SettingName, item.isChecked));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public interface Isettings
    {

    }
    [Serializable]
    public class settingValues<T> : Isettings
    {
        public string settingName;
        public T value;

        public settingValues(string setName, T value)
        {
            settingName = setName;
            this.value = value;
        }
    }

}
