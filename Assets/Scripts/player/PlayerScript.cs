using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using Cinemachine;
using static ResourceClass;

public class PlayerScript : NetworkBehaviour
{

    [SerializeField] private Camera childCamera;
    [SerializeField] private CinemachineVirtualCamera VCam;
    private CinemachineOrbitalTransposer transposer;

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
    [SyncVar]
    public NetworkIdentity CurrentSelectedListTop;
    public List<Selectable> Currently_Selected;
    public GameObject building;


    private static StandaloneInputModuleV2 currentInput;
    private StandaloneInputModuleV2 CurrentInput
    {
        get
        {
            if (currentInput == null)
            {
                if (GameController.UIInstance.GetComponent<StandaloneInputModuleV2>() != null)
                    currentInput = GameController.UIInstance.GetComponent<StandaloneInputModuleV2>() as StandaloneInputModuleV2;
                if (currentInput == null)
                {
                    Debug.LogError("Missing StandaloneInputModuleV2.");
                    // some error handling
                }
            }

            return currentInput;
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameController.addPlayer(netIdentity);
        if (!hasAuthority) return;
        if (!isLocalPlayer) { return; }
        if (World.main != null)
            World.main.Player = gameObject;
        childCamera = Camera.main;
        transposer = VCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        VCam.Priority = 1000;
        zoomLevel = defaultZoomLevel;
        //changeFOV(FOV);
        LocalPlayer();

        CmdPlaceFirstCity(transform.position);
    }

    private void LocalPlayer()
    {
        GameController.Instance.localSettings.localPlayer = this;
        GameController.Instance.localSettings.LocalCamera = Camera.main;

        FindObjectOfType<cloudmanager>().init(transform);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!hasAuthority) return;
        moveView();
        rotateView();
        zoomCamera();
        panView();
        heightAdjust();
    }
    private void heightAdjust()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 10000, 0), -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * 5f);
            //Debug.Log(hit.point);
        }
    }
    private void zoomCamera()
    {
        if (CurrentInput != null && (CurrentInput.GameObjectUnderPointer(-1) == null || !(CurrentInput.GameObjectUnderPointer(-1).layer == LayerMask.NameToLayer("UI"))))
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
        }
        //Vector3 height = childCamera.transform.position;
        //height.y = zoomLevel + transform.position.y;
        //childCamera.transform.position = height;
        //childCamera.transform.LookAt(transform);
        transposer.m_FollowOffset.y = (transform.position.y + zoomLevel);


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
                if (!hit.collider.CompareTag("Selectable"))
                    building.transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                var bb = building.GetComponent<BuildableObject>();
                if (bb.HasBeenBuild())
                {
                    bb.playerOwner = netIdentity;
                    building = null;
                }
                else
                {
                    SFXManagerController.Instance.Play("sfx_Error");
                }
                
            }
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(building);
                building = null;
            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            ray = childCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject(-1))
            {
                Debug.Log(hit.transform.name);
                if (!hit.collider.CompareTag("Selectable") && !(hit.transform.gameObject.layer == LayerMask.NameToLayer("UI")))
                {
                    Selectable[] tempList = new Selectable[Currently_Selected.Count];
                    Currently_Selected.CopyTo(tempList);
                    Currently_Selected.Clear();
                    foreach (var item in tempList)
                    {
                        item.unSelect();
                    }

                }
            }
        }
    }


    Ray ray;
    RaycastHit hit;
    private void Update()
    {
        if (!hasAuthority) return;
        BuildMode();
    }


    public void AddToSelection(Selectable ob)
    {
        if (Input.GetAxis("Speed") > 0)
        {
            ob.Select();
            Currently_Selected.Add(ob);
        }
        else
        {
            Selectable[] tempList = new Selectable[Currently_Selected.Count];
            Currently_Selected.CopyTo(tempList);
            Currently_Selected.Clear();
            Currently_Selected.Add(ob);
            ob.Select();
            foreach (var item in tempList)
            {
                item.unSelect();
            }
        }
        CmdChangeCurrentSelectedTopList(Currently_Selected[Currently_Selected.Count - 1].GetComponent<NetworkIdentity>());
    }

    [Command]
    public void CmdChangeCurrentSelectedTopList(NetworkIdentity to)
    {
        CurrentSelectedListTop = to;
    }


    #region CityBuilding
    [Command]
    public void CmdenterCityBuildMode(NetworkConnectionToClient conn = null)
    {
        GameController Instance = GameController.Instance;
        var ppcs = Instance.citySettings.perPlayerSettings[GameController.GetPlayerIndexbyNetID(conn.identity.netId)];
        if (ppcs.playerScript == null) return;
        GameObject go = Instantiate(Instance.citySettings.cityPrefab);
        NetworkServer.Spawn(go, conn);
        citySystem css = go.GetComponent<citySystem>();
        css.playerOwner = conn.identity;
        TargetBuildMode(conn, go.GetComponent<NetworkIdentity>());
    }
    [Command]
    public void CmdPlaceFirstCity(Vector3 pos, NetworkConnectionToClient conn = null)
    {
        GameController Instance = GameController.Instance;

        GameObject go = Instantiate(Instance.citySettings.cityPrefab);
        NetworkServer.Spawn(go, conn);
        citySystem css = go.GetComponent<citySystem>();
        css.playerOwner = conn.identity;
        go.transform.position = pos;
        if(Physics.Raycast(pos+(Vector3.up*100), -Vector3.up,out RaycastHit info))
        {
            go.transform.position = info.point;
        }
        css.HasBeenBuild();
    }

    #endregion

    #region Resourcebuilding
    [Command]
    public void CmdenterResourceBuildMode(int Resource, NetworkConnectionToClient conn = null)
    {
        GameController Instance = GameController.Instance;
        var ppcs = Instance.citySettings.perPlayerSettings[GameController.GetPlayerIndexbyNetID(conn.identity.netId)];
        if (ppcs.playerScript == null) return;
        ppcs.playerScript.CurrentSelectedListTop.TryGetComponent(out citySystem csr);
        if (csr == null) return;
        GameObject go = Instantiate(Instance.citySettings.ResourcePrefab);
        NetworkServer.Spawn(go, conn);
        ResourceBuildings css = go.GetComponent<ResourceBuildings>();
        css.type = (ResourceTypes)Resource;
        css.parent = csr.netIdentity;

        TargetBuildMode(conn, go.GetComponent<NetworkIdentity>());
        //ppcs.playerScript.buildObject(css);
    }

    #endregion
    [TargetRpc]
    private void TargetBuildMode(NetworkConnection conn, NetworkIdentity go)
    {
        GameController Instance = GameController.Instance;
        var ppcs = Instance.citySettings.perPlayerSettings[Instance.localPlayerID];
        ppcs.playerScript.buildObject(go.GetComponent<BuildableObject>());
    }
}
