using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LocalizationEditor : EditorWindow
{
    string filePath = "Assets/scripts/Localization/locale";
    bool filePathsLoaded = false;
    List<TextAsset> localFiles = new List<TextAsset>();
    

    [MenuItem("Window/LocalizationTool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LocalizationEditor));
    }
    void OnGUI()
    {
        if (!filePathsLoaded)
        {
            TextAsset[] array = (TextAsset[])Resources.LoadAll(filePath);
            for (int i = 0; i < array.Length; i++)
            {
                TextAsset item = array[i];
                localFiles.Add(item);
                Debug.Log(item.text);
            }
            foreach (var item in localFiles)
            {
                if (item != null)
                Debug.Log(item.text);
            }


            filePathsLoaded = true;
        }

    }
}
