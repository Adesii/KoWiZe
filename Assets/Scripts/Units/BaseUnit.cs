using System;
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


[System.Serializable]
public abstract class BaseUnit : IUnit
{
    public Image UnitIcon;
    public string Unit_name = "base";
    public unit_subtype subtype;
    public float hp = 100f;
    public float armor = 1f;
    public float attack = 10f;
    public float speed = 100f;
    public Dictionary<ResourceClass.ResourceTypes, float> costs;
    public unit_building building;
    public float build_time = 10f;
    public float UnitRange = 1f;
    //todo technology unlock
    [Multiline]
    public string description = "Lorem Ipsum Dolor Sit Amet";
    [SerializeField]
    List<UnitStrenghts> unitStrenghts;
    [Serializable]
    public struct UnitStrenghts
    {
        public string unitName;
        public float UnitStrenght;
    }
    

    public virtual void TakeDamage(float amount)
    {

    }
    public virtual void MoveUnit()
    {

    }
    public virtual void Attack(float amount)
    {

    }
    public override string ToString()
    {
        return Unit_name;
    }
}
[System.Serializable]
public class MeleeUnit : BaseUnit
{
}
[System.Serializable]
public class RangedUnit : BaseUnit
{


}
[System.Serializable]
public class SiegeUnit : BaseUnit
{


}
