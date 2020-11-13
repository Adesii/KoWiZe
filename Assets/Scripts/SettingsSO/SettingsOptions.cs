using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName ="SettingsSetting")]
[ExecuteAlways]
public class SettingsOptions : ScriptableObject
{
    public GameObject SettingsPrefab;
    public List<UniversalRenderPipelineAsset> PredefinedSettings;
    [Reorderable]
    public List<settingSetting> whatSettingsToUse;

    public void PrintSettings()
    {
        //fillList();
    }

    public void fillList()
    {
        whatSettingsToUse = new List<settingSetting>();
        foreach (var item in PredefinedSettings[0].GetType().GetProperties())
        {
            settingSetting temp = new settingSetting() {
                SettingName = item.Name
            };
            whatSettingsToUse.Add(temp);
        }
    }

    public enum settingsType
    {
        dropDown,
        slider,
        checkBox
    }
    [Serializable]
    [ExecuteInEditMode]
    public class settingSetting
    {
        public string SettingName;
        public string Categorie;
        public settingsType typeofSetting;
        public int value;
        public int max;
        public int min;
        public bool isChecked;
        [Reorderable]
        public List<dropdownSettings> dropDownOptions;
        [Serializable]
        public class dropdownSettings
        {
            public string name;
            public int numberValue;
            public string stringValue;
            public bool boolValue;
        }

    }

    public GameObject GetSettingsPane(int ID)
    {

        return null;
    }



    GameObject setupGameObject(GameObject prefab,int ID)
    {
        return prefab;
    }
}
