﻿using System;
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
    [Range(0.1f, 100)]
    public float RotateSens = 10f;

    [Header("Camera Settings")]
    [Range(70, 103)]
    public float FOV = 80f;




    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;



    [Header("Current Selection Settings")]
    public List<GameObject> Currently_Selected;
    public GameObject building; 

    // Start is called before the first frame update
    void Start()
    {
        GameController.addPlayer(this);
        zoomLevel = defaultZoomLevel;
        changeFOV(FOV);
    }

    // Update is called once per frame
    void LateUpdate()
    {

        moveView();
        rotateView();
        zoomCamera();
        panView();
        heightAdjust();
    }
    private void heightAdjust()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 10000, 0), -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * 5f);
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
        if (axisX != 0)
        {
            Translation.x = axisX * moveSpeed * (zoomLevel / 10);
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
        transform.Translate(Translation * Time.deltaTime, Space.Self);
    }
    private void panView()
    {




        if (Input.GetMouseButtonDown(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);


            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }

            /*
            origin = hit.point;
            Debug.Log(origin);
            transformOrigin = transform.position;
            */

        }
        if (Input.GetMouseButton(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);


            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                transform.position = transform.position + dragStartPosition - dragCurrentPosition;
            }


            /*
            Difference = hit.point;
            origin.y = 0;
            Difference.y = 0;
            transformOrigin.y = 0;
            Debug.Log(origin + ";;;" + Difference + ";;;" + transformOrigin);
            transform.position = transform.position + (origin - Difference);
            */
        }

    }

    private void changeFOV(float FOVNumber)
    {
        childCamera.fieldOfView = FOVNumber;
    }

    public bool buildObject(BuildableObject building)
    {

        if (this.building != null) return false;

        building.wantsTobeBuild();
        this.building = building.gameObject;

        return true;
    }

    public void BuildMode()
    {
        if (building != null)
        {
            ray = childCamera.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit))
            {
                building.transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                building.GetComponent<BuildableObject>().HasBeenBuild();
                building = null;
            }
        }
    }


    Ray ray;
    RaycastHit hit;
    private void Update()
    {
        BuildMode();
    }
}