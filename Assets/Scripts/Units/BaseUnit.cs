using System;
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
[System.Serializable]
public abstract class AORQueableItem
{
    [Newtonsoft.Json.JsonIgnore]
    public Sprite UnitIcon;
    [HideInInspector]
    [Newtonsoft.Json.JsonIgnore]
    public citySystem ownerCity;
    public string Unit_name = "base";
    public float build_time = 10f;
    [HideInInspector]
    public int QueueType = -1;
    public virtual float GetBuildTime()
    {
        return build_time;
    }
}


[System.Serializable]
public abstract class BaseUnit : AORQueableItem,IUnit
{
    public unit_subtype subtype;
    public float hp = 100f;
    public float armor = 1f;
    public float attack = 10f;
    public float speed = 100f;
    public List<Costs> costs;
    public unit_building building;

    [HideInInspector]
    public string Categorie = "Not Assigned";
    
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
        public override string ToString()
        {
            return $"{unitName} = {UnitStrenght}";
        }
    }
    [Serializable]
    public struct Costs
    {
        public ResourceClass.ResourceTypes Resource;
        public float Cost;
        public override string ToString()
        {
            return $"{Resource} = {Cost}";
        }
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

    public virtual Dictionary<string,object> GetStats()
    {
        return new Dictionary<string, object>
        {
            { "Unit_Costs", costs },
            { "Unit_Subtype", subtype },
            { "Unit_HP", hp },
            { "Unit_Armor", armor },
            { "Unit_AttackDamage", attack },
            { "Unit_MovementSpeed", speed },
            {"Unit_BuildTime",build_time },
            {"Unit_Range",UnitRange },
            //{"Unit_Strenghts",unitStrenghts }

        };
    }
    public override string ToString()
    {
        return Unit_name;
    }
    public static bool isUnit(AORQueableItem i)
    {
        if(i.QueueType == 1)
        {
            return true;
        }
        return false;
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
