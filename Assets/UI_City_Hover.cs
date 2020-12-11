using System.Collections.Generic;
using UnityEngine;
using static ResourceClass;

public class UI_City_Hover : MonoBehaviour
{
    public GameObject CityHoverIcon;
    public GameObject BuildingPanel;
    public Dictionary<citySystem, UI_City_Hover_prefab_Store> HoverList = new Dictionary<citySystem, UI_City_Hover_prefab_Store>();
    public float UIScale = 1f;

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
            
    }

    // Update is called once per frame
    void LateUpdate()
    {
        updateHover();
    }

    private void updateHover()
    {
        foreach (var item in HoverList)
        {
            if (!item.Value.gameObject.activeSelf) continue;
            if (cam == null) cam = GameController.Instance.localSettings.LocalCamera;
            item.Value.transform.position = cam.WorldToScreenPoint(item.Key.hoverCityPosition.position);
            item.Value.sortingStuff.sortingOrder = (int)-(Vector3.Distance(cam.transform.position, item.Key.transform.position));
            item.Value.UIscale = UIScale;


        }
    }
    public static UI_City_Hover_prefab_Store addNewCity(citySystem city)
    {
        UI_City_Hover_prefab_Store gm = Instantiate(_Instance.CityHoverIcon, _Instance.transform).GetComponent<UI_City_Hover_prefab_Store>();
        _Instance.HoverList.Add(city, gm);
        gm.ownCity = city;
        return gm;
    }
    public void BuildResourceBuilding(int resourcesTypes)
    {
        Debug.Log((ResourceTypes)resourcesTypes);
    }
}
