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

        }
    }

    public void Attack()
    {
        startMove = true;
    }
}
