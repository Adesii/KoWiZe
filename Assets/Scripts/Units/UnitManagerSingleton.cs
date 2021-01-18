using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yaSingleton;
using UnityEditor;
[CreateAssetMenu(fileName = "UnitManager", menuName = "KoWiZe Custom Assets/Singletons/UnitManager")]
[System.Serializable]
public class UnitManagerSingleton : LazySingleton<UnitManagerSingleton>
{
    [SerializeField]
    private RangedUnit[] rangedUnits;
    [SerializeField]
    private MeleeUnit[] meleeUnits;
    [SerializeField]
    private SiegeUnit[] siegeUnits;

    public RangedUnit[] RangedUnits { get => rangedUnits; set => rangedUnits = value; }
    public MeleeUnit[] MeleeUnits { get => meleeUnits; set => meleeUnits = value; }
    public SiegeUnit[] SiegeUnits { get => siegeUnits; set => siegeUnits = value; }
}
