#if UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(TechTreeManager))]
public class AORTechTreeEditor : Editor
{
    public static string TechTreePath { get => (Application.persistentDataPath + "/tech"); }
    public static string TechTreeFilePath { get => (TechTreePath + "/TechTreeData.tech"); }

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
        string txt = File.ReadAllText(TechTreeFilePath);
        TechTreeManager ui = (TechTreeManager)serializedObject.targetObject;
        ui.Techs = JsonUtility.FromJson<List<TechTree>>(txt);
    }

    public void SaveUnits()
    {
        File.WriteAllText(TechTreeFilePath, JsonConvert.SerializeObject(((TechTreeManager)serializedObject.targetObject).Techs));
    }
}
#endif

