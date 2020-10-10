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
    public float zoomLevel = 30f;
    [Range(0.1f, 100)]
    public float zoomSens = 10f;

    public float maxZoom = 120f;
    public float minZoom = 10f;
    

    [Header("Rotation Settings")]
    [Range(0.1f,100)]
    public float RotateSens = 10f;

    [Header("Camera Settings")]
    [Range(70,103)]
    public float FOV = 80f;

    // Start is called before the first frame update
    void Start()
    {
        zoomLevel = defaultZoomLevel;
        changeFOV(FOV);
    }

    Vector3 origin;
    Vector3 Difference;
    Vector3 transformOrigin;
    // Update is called once per frame
    void LateUpdate()
    {
        
        moveView();
        rotateView();
        zoomCamera();
        Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,10000))
        {
            if (hit.transform.position.x - transform.position.x < 30*zoomLevel && hit.transform.position.z - transform.position.z < 20*zoomLevel)
            {

                panView(hit);
            }
           
        }
        heightAdjust();
    }
    private void heightAdjust()
    {
        Ray ray = new Ray(transform.position+new Vector3(0,10000,0), -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = Vector3.Lerp(transform.position,hit.point, Time.deltaTime * 5f);
            //Debug.Log(hit.point);
        }
    }
    private void zoomCamera()
    {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && zoomLevel < maxZoom)
            {

                if (Input.GetAxis("Speed") != 0)
                {
                    zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSens * 2;
                }
                else
                {
                    zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSens;
                }

            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0 && zoomLevel > minZoom)
            {
                if (Input.GetAxis("Speed") != 0)
                {
                    zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSens * 2;
                }
                else
                {
                    zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSens;
                }

            }

            Vector3 height = childCamera.transform.position;
            height.y = zoomLevel + transform.position.y;
            childCamera.transform.position = height;
            childCamera.transform.LookAt(transform);
        
        
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
            Translation.x = axisX * moveSpeed *(zoomLevel/10);
        }
        if (axisZ != 0)
        {
            Translation.z = axisZ * moveSpeed * (zoomLevel / 10);
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

    private void changeFOV(float FOVNumber)
    {
        childCamera.fieldOfView = FOVNumber;
    }


}
