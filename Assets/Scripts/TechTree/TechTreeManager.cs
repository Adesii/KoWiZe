using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yaSingleton;

[CreateAssetMenu(fileName ="TechTreeManager",menuName = "KoWiZe Custom Assets/Singletons/TechTreeManager")]
public class TechTreeManager : Singleton<TechTreeManager>
{
    public List<TechTree> Techs;
}
[System.Serializable]
public abstract class TechNode : ScriptableObject
{
    public string TechName = "Nothing";
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
