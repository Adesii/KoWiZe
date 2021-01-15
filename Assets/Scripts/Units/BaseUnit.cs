using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum unit_type
{
    Melee,
    Ranged,
    Siege
}
public enum unit_subtype
{
    None,
    Cavalry
}

public enum unit_building
{
    Barracks,
    Archery_Range,
    Stable,
    Workshop
}

public interface IUnit
{
}
public abstract class BaseUnit : ScriptableObject, IUnit
{
    public Image UnitIcon;
    public string Unit_name = "base";
    public unit_type type;
    public unit_subtype subtype;
    public float hp = 100f;
    public float armor = 1f;
    public float attack = 10f;
    public float speed = 100f;
    public Dictionary<ResourceClass.ResourceTypes, float> costs;
    public unit_building building;
    public float build_time = 10f;
    //todo technology unlock
    [Multiline]
    public string description = "Lorem Ipsum Dolor Sit Amet";

    public Dictionary<IUnit, float> unit_strenght;


    public void TakeDamage(float amount)
    {

    }
    public void MoveUnit()
    {
        
    }
    public void Attack(float amount)
    {

    }
}
[CreateAssetMenu(fileName = "MeeleUnit", menuName = "KoWiZe Custom Assets/Units/Melee")]
public class MeleeUnit : BaseUnit
{


}
[CreateAssetMenu(fileName ="RangedUnit", menuName ="KoWiZe Custom Assets/Units/Ranged" )]
public class RangedUnit : BaseUnit
{
    public float UnitRange = 100f;

}

[CreateAssetMenu(fileName="SiegeUnit",menuName="KoWiZe Custom Assets/Units/Siege")]
public class SiegeUnit : BaseUnit
{


}
