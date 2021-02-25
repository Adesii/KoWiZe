using FMODUnity;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AORUnitArmy : NetworkBehaviour
{
    [SyncVar]
    public PlayerScript.AttackOrder order;
    bool startMove = false;

    private void Update()
    {
        if (startMove) move();
    }
    public void move()
    {
        transform.position = Vector3.MoveTowards(transform.transform.position, order.enemycity.transform.position, Time.deltaTime * order.movespeed);
        if (Vector3.Distance(transform.position, order.enemycity.transform.position) < 10f)
        {
            startMove = false;
            CmdEvaluate();
        }
    }
    [Command(ignoreAuthority =true)]
    public void CmdEvaluate()
    {
        List<BaseUnit> CityunitList = new List<BaseUnit>();
        List<BaseUnit> ArmyunitList = new List<BaseUnit>();
        var cityLifePool = 0f;
        var armyLifePool = 0f;

        var cityAttackPool = 0f;
        var armyAttackPool = 0f;

        var cityArmorPool = 0f;
        var armyArmorPool = 0f;


        foreach (var item in order.enemycity.GetComponent<citySystem>().unitInvertoryNameList)
        {
            if(UnitManagerSingleton.Instance.AllBaseUnits.TryGetValue(item, out BaseUnit unit))
            {
                cityLifePool += unit.hp;
                cityAttackPool += unit.attack;
                cityArmorPool += unit.armor;
                CityunitList.Add(unit);
            }
        }
        foreach (var item in order.unitList)
        {
            if (UnitManagerSingleton.Instance.AllBaseUnits.TryGetValue(item, out BaseUnit unit))
            {
                armyLifePool += unit.hp;
                armyAttackPool += unit.attack;
                armyArmorPool += unit.armor;
                ArmyunitList.Add(unit);
            }
        }

        cityLifePool += cityArmorPool * 1.3f;
        armyLifePool += armyArmorPool * 1.2f;

        cityLifePool += cityAttackPool * 0.7f;
        armyLifePool += armyAttackPool * 0.5f;
        if (cityLifePool <= armyLifePool)
        {
            Debug.Log("Attacker Won");
        }
        else
        {
            Debug.Log("Defense Won");
        }
        Debug.Log(armyLifePool);
        Debug.Log(cityLifePool);

    }


    public void Attack()
    {
        startMove = true;
    }
}
