using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class planeCamera : MonoBehaviour
{

    public Camera childCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float defaultZoomLevel = 30f;
    public float zoomSens = 10f;

    [Header("Rotation Settings")]
    public float RotateSens = 10f;

    [Header("Camera Settings")]
    public float FOV = 80f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 origin;
    Vector3 Difference;
    Vector3 transformOrigin;
    // Update is called once per frame
    void LateUpdate()
    {
        moveView();
        rotateView();
        Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,100))
        {
            panView(hit);
        }
    }

    private void rotateView()
    {
        float Rotate = Input.GetAxis("RotateView");
        Vector3 Translation = Vector3.zero;
        if (Rotate != 0)
        {
            Translation.y = Rotate * RotateSens;
        }
        if (Input.GetAxis("Speed") != 0)
        {
            Translation.y *= 2;
        }
        transform.Rotate(Translation * Time.deltaTime, Space.Self);
    }

    private void moveView()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisZ = Input.GetAxis("Vertical");
        Vector3 Translation = Vector3.zero;
        if (axisX!=0)
        {
            Translation.x = axisX * moveSpeed;
        }
        if (axisZ != 0)
        {
            Translation.z = axisZ * moveSpeed;
        }
        if (Input.GetAxis("Speed") != 0)
        {
            Translation.z *= 2;
            Translation.x *= 2;
        }
        transform.Translate(Translation*Time.deltaTime, Space.Self);
    }
    private void panView(RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(2))
        {
            origin = hit.point;
            Debug.Log(origin);
            transformOrigin = transform.position;

        }
        if (Input.GetMouseButton(2))
        {
            Difference = hit.point;
            origin.y = 0;
            Difference.y = 0;
            transformOrigin.y = 0;
            Debug.Log(origin + ";;;" + Difference + ";;;" + transformOrigin);
            transform.position = transform.position + (origin - Difference);

        }
    }


}
