using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ResourceClass;
using DG.Tweening;

public class UI_City_Hover : MonoBehaviour
{
    public GameObject CityHoverIcon;
    public GameObject BuildingPanel;
    public Dictionary<citySystem, UI_City_Hover_prefab_Store> HoverList = new Dictionary<citySystem, UI_City_Hover_prefab_Store>();
    public float UIScale = 1f;

    Vector3 lastPos = new Vector3();

    private Camera cam;

    public static UI_City_Hover _Instance;

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        cam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if(Vector3.SqrMagnitude(lastPos- cam.transform.position)>1f)
        {
            updateHover();
            lastPos = cam.transform.position;
        }

    }
    private void updateHover()
    {
        foreach (var item in HoverList)
        {
            //if (!item.Value.gameObject.activeInHierarchy) continue;
            if (item.Key == null) continue;
            if (cam == null) cam = GameController.Instance.localSettings.LocalCamera;
            if (!(item.Value.transform as RectTransform).IsVisibleFrom())
            {
                item.Value.gameObject.SetActive(false);
            }
            else
            {
                item.Value.gameObject.SetActive(true);

            }
            item.Value.transform.DOMove(cam.WorldToScreenPoint(item.Key.hoverCityPosition.position),0.05f);
            item.Value.sortingStuff.sortingOrder = (int)-(Vector3.Distance(cam.transform.position, item.Key.transform.position));
            item.Value.UIscale = UIScale;
            var val = (Mathf.Lerp(1f, 0.1f, Vector3.Distance(item.Key.transform.position,cam.transform.position)/400f));
            //print(val);
            item.Value.transform.localScale = new Vector3(val,val,val);
        }
    }
    public static UI_City_Hover_prefab_Store addNewCity(citySystem city)
    {
        UI_City_Hover_prefab_Store gm = Instantiate(_Instance.CityHoverIcon, _Instance.transform).GetComponent<UI_City_Hover_prefab_Store>();
        _Instance.HoverList.Add(city, gm);
        gm.UIscale = _Instance.UIScale;
        gm.ownCity = city;
        gm.SetName();
        _Instance.updateHover();
        return gm;
    }
    public void BuildResourceBuilding(int resourcesTypes)
    {
        Debug.Log((ResourceTypes)resourcesTypes);
    }
}
