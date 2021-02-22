using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using yaSingleton;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "TechTreeManager", menuName = "KoWiZe Custom Assets/Singletons/TechTreeManager")]
public class TechTreeManager : Singleton<TechTreeManager>
{
    public bool SaveData;
    [SerializeField]
    public List<TechTree> Techs;

    public Action NewTechUnlocked;


    public Dictionary<string, TechNode> TechNodeDict;

    public List<TechTree> saveToChangeTechs;

    public static string TechTreePath { get => (Application.persistentDataPath + "/tech"); }
    public static string TechTreeFilePath { get => (TechTreePath + "/TechTreeData.tech"); }

    public static bool CreateNewInstance = true;
    protected override void Initialize()
    {
        if (CreateNewInstance)
        {
            base.Initialize();

            CreateNewInstance = false;
            var b = Instantiate(this);
            saveToChangeTechs = b.Techs;
            TechNodeDict = new Dictionary<string, TechNode>();
            foreach (var item in saveToChangeTechs)
            {
                foreach (var layer in item.techLayers)
                {
                    foreach (var node in layer.techNodes)
                    {
                        TechNodeDict.Add(node.TechName, node);
                        if (node.isUnlocked)
                        {
                            foreach (var unitsIDs in node.UnitIDS)
                            {
                                if (UnitManagerSingleton.Instance.AllBaseUnits.TryGetValue(unitsIDs, out BaseUnit unit))
                                    GameController.Instance.localSettings.LocalPlayerUnlockedUnits.Add(unit);
                            }
                        }


                    }
                }
            }
        }

    }
    protected override void Deinitialize()
    {
        if (CreateNewInstance)
            base.Deinitialize();
        saveToChangeTechs = new List<TechTree>();
        CreateNewInstance = true;
    }

    public void unlockedTech(TechNode tech)
    {
        foreach (var item in TechNodeDict)
        {
            for (int i = 0; i < item.Value.dependsIDs.Count; i++)
            {
                TechNode.Dependencies dependencies = item.Value.dependsIDs[i];
                if (dependencies.techName.Equals(tech.TechName))
                {

                    dependencies.isUnlocked = true;
                }
                item.Value.dependsIDs[i] = dependencies;
            }
            if (item.Value.dependsIDs.All((e) => e.isUnlocked))
            {
                item.Value.isAvailable = true;
            }
        }
        foreach (var item in tech.UnitIDS)
        {
            if (UnitManagerSingleton.Instance.AllBaseUnits.TryGetValue(item, out BaseUnit unit))
                GameController.Instance.localSettings.LocalPlayerUnlockedUnits.Add(unit);
        }

        NewTechUnlocked?.Invoke();
    }
}
[System.Serializable]
public class TechNode
{

    public GameObject owner;
    public string TechName = "Nothing";
    [Multiline]
    public string TechDescription = "base";

    public List<Dependencies> dependsIDs;

    public List<string> UnitIDS;

    [SerializeField]
    private bool m_isAvailable = false;

    public bool isAvailable
    {
        get
        {
            return m_isAvailable;
        }
        set
        {
            m_isAvailable = value;
            if (value) onAvailable?.Invoke();
        }
    }
    [SerializeField]
    private bool m_unlocked = false;

    public bool isUnlocked
    {
        get
        {
            return m_unlocked;
        }
        set
        {
            m_unlocked = value;
            if (value && TechTreeManager.Instance != null) { TechTreeManager.Instance.unlockedTech(this); onUnlocked?.Invoke(this); };
        }
    }
    [Serializable]
    public struct Dependencies
    {
        public bool isUnlocked;
        public string techName;
    }

    public TechUnlock onUnlocked;
    public Action onAvailable;

    [Serializable]
    public class TechUnlock : UnityEvent<TechNode>
    {
    }
    public override string ToString()
    {
        return TechName;
    }
}
[System.Serializable]
public class TechLayer
{
    public string LayerName = "Layer";
    public List<TechNode> techNodes;
    public override string ToString()
    {
        return LayerName;
    }
}
[System.Serializable]
public class TechTree
{
    public string TechTreeName = "TechTree";
    public List<TechLayer> techLayers;
    public override string ToString()
    {
        return TechTreeName;
    }
}
