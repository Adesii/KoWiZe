using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yaSingleton;
using UnityEditor;
[CreateAssetMenu(fileName = "UnitManager", menuName = "KoWiZe Custom Assets/Singletons/UnitManager")]
[System.Serializable]
[CustomEditor(typeof(unitManagerWindow))]
public class UnitManagerSingleton : LazySingleton<UnitManagerSingleton>
{
    public RangedUnit[] RangedUnits;
    public RangedUnit[] MeleeUnits;
    public RangedUnit[] SiegeUnits;
}
