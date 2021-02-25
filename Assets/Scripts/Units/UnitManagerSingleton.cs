using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yaSingleton;
using UnityEditor;
[CreateAssetMenu(fileName = "UnitManager", menuName = "KoWiZe Custom Assets/Singletons/UnitManager")]
[ExecuteAlways]
[System.Serializable]
public class UnitManagerSingleton : LazySingleton<UnitManagerSingleton>
{
    public RangedUnit[] rangedUnits;
    public MeleeUnit[] meleeUnits;
    public SiegeUnit[] siegeUnits;
    public GameObject UnitArmyPrefab;

    public Dictionary<string, BaseUnit[]> AllUnits;
    protected override void Initialize()
    {
        base.Initialize();
        AllUnits = new Dictionary<string, BaseUnit[]>
            {
                { "Ranged Units", rangedUnits },
                { "Melee Units", meleeUnits },
                { "Siege Units", siegeUnits }
            };
        var b = new Dictionary<string, BaseUnit>();

        foreach (var item in AllUnits)
        {
            foreach (var unit in item.Value)
            {
                unit.Categorie = item.Key;
                b.Add(unit.Unit_name, unit);
            }
        }
        AllBaseUnits = b;
    }
    public Dictionary<string, BaseUnit> AllBaseUnits = new Dictionary<string, BaseUnit>();
}
