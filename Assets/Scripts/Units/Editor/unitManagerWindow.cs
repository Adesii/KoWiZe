#if UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(UnitManagerSingleton))]
public class unitManagerWindow : Editor
{
    string SaveFile = "Assets/Scripts/Units/unitSaveData.json";
    Dictionary<string,IUnit[]> units;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Units"))
        {
            Debug.Log("saved");
            SaveUnits();
        }
        if (GUILayout.Button("Load Unity"))
        {
            Debug.Log("load");
            LoadUnits();
        }

    }

    public void LoadUnits()
    {
        string txt = File.ReadAllText(SaveFile);
        var ob = JObject.Parse(txt);
        int i = 0;
        units = new Dictionary<string, IUnit[]>();
        foreach (var item in ob)
        {
            if (item.Key.Contains("Units"))
            {
                switch (i)
                {

                    case 0:
                        units.Add(typeof(RangedUnit[]).ToString(), item.Value.ToObject<RangedUnit[]>());
                        break;
                    case 1:
                        units.Add(typeof(MeleeUnit[]).ToString(), item.Value.ToObject<MeleeUnit[]>());
                        break;
                    case 2:
                        units.Add(typeof(SiegeUnit[]).ToString(), item.Value.ToObject<SiegeUnit[]>());
                        break;
                    default:
                        Debug.LogError("Units of type: " + item.Value + " not added yet to loading Methods");
                        break;
                }
                i++;
            }
        }
        UnitManagerSingleton ui = (UnitManagerSingleton)serializedObject.targetObject;

        ui.meleeUnits = (MeleeUnit[])units[ui.meleeUnits.GetType().ToString()];
        ui.rangedUnits = (RangedUnit[])units[ui.rangedUnits.GetType().ToString()];
        ui.siegeUnits = (SiegeUnit[])units[ui.siegeUnits.GetType().ToString()];
    }

    public void SaveUnits()
    {
        File.WriteAllText(SaveFile, JsonConvert.SerializeObject(serializedObject.targetObject));
    }
}
#endif

