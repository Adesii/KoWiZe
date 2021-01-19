using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yaSingleton;
using UnityEditor;
[CreateAssetMenu(fileName = "UnitManager", menuName = "KoWiZe Custom Assets/Singletons/UnitManager")]
[ExecuteAlways]
[System.Serializable]
public class UnitManagerSingleton : Singleton<UnitManagerSingleton>
{
    public RangedUnit[] rangedUnits;
    public MeleeUnit[] meleeUnits;
    public SiegeUnit[] siegeUnits;

}
