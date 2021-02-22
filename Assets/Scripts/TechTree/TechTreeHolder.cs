using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "holder", menuName = "KoWiZe Custom Assets/Singletons/TechTreeHolder")]
public class TechTreeHolder : ScriptableObject
{
    public List<TechTree> Tree;
}
