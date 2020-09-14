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
    [Tooltip("Mouse Sensitivity.")]
    public float mouseSens = 1.0f;

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
    void Update()
    {
        Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
        
        if (Input.GetMouseButtonDown(0))
        {
            origin = hit.point;
            Debug.Log(origin);
            transformOrigin = transform.position;

        }
        if (Input.GetMouseButton(0))
        {
            Difference = hit.point;
            origin.y = 0;
            Difference.y = 0;
            transformOrigin.y = 0;
            Debug.Log(origin+";;;"+Difference+";;;"+transformOrigin);
            transform.position = transform.position+(origin - Difference);

        }
        }
    }


}
