using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Assertions;

public class LocalizationEditor : EditorWindow
{
    string filePath = "locale";
    string search = "";
    bool filePathsLoaded = false;
    CSVLoader loader = new CSVLoader();
    TextAsset[] foundLanguagesAssets;
    Dictionary<string, Dictionary<string, string>> languagesKeeper;
    Dictionary<string, Dictionary<string, string>> languagesKeeperEdited;
    Dictionary<string, bool> foldoutlist;

    Vector2 scrollPos = new Vector2();

    string newKey = "";


    [MenuItem("Window/LocalizationTool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LocalizationEditor));
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Files")) { reloadFiles(); filePathsLoaded = false; }
        search = GUILayout.TextField(search,GUILayout.MinWidth(position.width/3));
        if (GUILayout.Button("save Changes")) Debug.Log(saveFiles());
        GUILayout.EndHorizontal();
        if (languagesKeeper == null) filePathsLoaded = false;

        if (!filePathsLoaded)
        {
            reloadFiles();
        }
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (var d in languagesKeeper[languagesKeeper.Keys.First()].Keys.ToArray())
        {
            
            bool searchFound = false;
            foreach (var lang in languagesKeeper.Keys.ToArray())
            {
                string temp;
                languagesKeeperEdited[lang].TryGetValue(d, out temp);
                if (d.ToLower().Contains(search.ToLower()) || temp.ToLower().Contains(search) || search == "") searchFound = true;
            }
            if (searchFound)
            {
                bool foldout = false;
                foldoutlist.TryGetValue(d, out foldout);
                GUILayout.BeginHorizontal();
                
                GUILayout.BeginVertical();
                
                EditorGUILayout.GetControlRect(true, 16f, EditorStyles.foldout);
                Rect foldRect = GUILayoutUtility.GetLastRect();
                GUI.Box(foldRect,"");
                if (Event.current.type == EventType.MouseUp && foldRect.Contains(Event.current.mousePosition))
                {
                    foldout = !foldout;
                    GUI.changed = true;
                    Event.current.Use();
                }
                foldout = EditorGUI.Foldout(foldRect, foldout, d);
                EditorGUI.indentLevel++;
                if (foldout)
                {
                    EditorGUI.indentLevel++;
                    foreach (var item in languagesKeeper)
                    {
                        GUILayout.Label("Language:" + item.Key);
                        EditorGUI.indentLevel++;

                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                        string edited = GUILayout.TextArea(languagesKeeperEdited[item.Key][d], GUILayout.MinHeight(position.height / item.Value.Count));
                        GUILayout.EndHorizontal();
                        languagesKeeperEdited[item.Key][d] = edited;
                        EditorGUI.indentLevel--;

                    }
                    EditorGUI.indentLevel--;
                }

                foldoutlist[d] = foldout;
                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
                if (GUILayout.Button("x",GUILayout.Width(position.width/30),GUILayout.Height(position.height/30))) { deleteEntry(d); }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(position.height / 100);
        }
        GUILayout.EndScrollView();

        GUILayout.Label("Create new Key");
        GUILayout.BeginHorizontal();
        newKey = GUILayout.TextField(newKey);
        if (GUILayout.Button("Create",GUILayout.MinWidth(position.width/4),GUILayout.MaxWidth(position.width/4))) if(!string.IsNullOrWhiteSpace(newKey)) addNewKey(newKey);
        GUILayout.EndHorizontal();
    }

    private void reloadFiles()
    {
        AssetDatabase.Refresh();
        languagesKeeper = new Dictionary<string, Dictionary<string, string>>();
        foundLanguagesAssets = null;
        foundLanguagesAssets = Resources.LoadAll<TextAsset>(filePath);
        for (int i = 0; i < foundLanguagesAssets.Length; i++)
        {
            TextAsset item = foundLanguagesAssets[i];
            languagesKeeper.Add(item.name, loader.GetDictionaryFromFile(item));
        }
        languagesKeeperEdited = new Dictionary<string, Dictionary<string, string>>(languagesKeeper);
        filePathsLoaded = true;
        foldoutlist = new Dictionary<string, bool>();
        foreach (var item in languagesKeeper)
        {
            foldoutlist.Add(item.Key, false);
        }
    }

    void addNewKey(string name)
    {
        foreach (var item in languagesKeeper.Keys.ToArray())
        {
            languagesKeeper[item].Add(name, "");
        }
        saveFiles();
        newKey = "";
    }

    bool saveFiles()
    {
        for (int i = 0; i < foundLanguagesAssets.Length; i++)
        {
            TextAsset localeFIle = foundLanguagesAssets[i];
            foreach (var lang in languagesKeeperEdited)
            {
                if (localeFIle.name == lang.Key)
                {
                    File.WriteAllText(AssetDatabase.GetAssetPath(localeFIle), dictToCSV(lang.Value,lang.Key),System.Text.Encoding.UTF8);
                }
            }
        }
        reloadFiles();
        return true;
    }
    void deleteEntry(string index)
    {
        foreach (var item in languagesKeeper.Keys.ToArray())
        {
            languagesKeeper[item].Remove(index);
            languagesKeeperEdited[item].Remove(index);
        }
        Debug.Log(index);
    }
    string dictToCSV(Dictionary<string,string> dic,string lang)
    {
        string temp = "\"key\",\""+lang+"\",\n";
        int i = 0;
        foreach (var item in dic)
        {
            if(i < dic.Count-1)
            temp += "\"" + item.Key + "\",\"" + item.Value + "\",\n";
            else
            {
                temp += "\"" + item.Key + "\",\"" + item.Value + "\",";
            }
            i++;
        }
        return temp;
    }
}
