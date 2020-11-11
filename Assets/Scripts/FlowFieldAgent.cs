using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldAgent : MonoBehaviour
{
    public GameObject goal;
    public Vector3 velocity;
    public float gravity;
    [Header("Movement Settings")]
    public float rotationSpeed = 20f;
    public float accelaration;
    public float deaccelartion;
    public float maxMovementSpeed;
    public float maxRotationAngle;


    CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        moveToGoal();

    }
    void moveToGoal()
    {
        rotateBody();
        moveBody();   
    }
    void rotateBody()
    {
        cc.transform.rotation =  Quaternion.Euler(0,Quaternion.RotateTowards(cc.transform.rotation, Quaternion.LookRotation((cc.transform.position - goal.transform.position).normalized*-1), rotationSpeed*Time.deltaTime).eulerAngles.y,0);
    }
    void moveBody()
    {
        
        float angle = 0;
        Quaternion.FromToRotation((cc.transform.position - goal.transform.position).normalized * -1, cc.transform.forward).ToAngleAxis(out angle,out _);
        Debug.Log(angle);
        if ( angle<maxRotationAngle)
        {
            if(velocity.magnitude<maxMovementSpeed)
            {
                velocity += Vector3.forward * accelaration * Time.deltaTime;
            }
            

        }
        else
        {
                velocity = Vector3.Lerp(velocity,Vector3.zero, deaccelartion * Time.deltaTime);
        }
        cc.Move(transform.TransformDirection(velocity*Time.deltaTime));
    }
}

