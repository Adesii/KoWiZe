using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string description = "Lorem Ipsum Dolor Sit Amet";
    public Dictionary<IUnit, float> unit_strenght;
}
