using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using yaSingleton;
using static ResourceClass;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameController", menuName = "KoWiZe Custom Assets/Singletons/GameController")]
public class GameController : Singleton<GameController>
{
    private int currLangIndex;
    public Languages currentLanguage = Languages.en;
    public static event Action languageChangeEvent;
    public GameObject UI_Prefab;
    public int localPlayerID = 0; 
    public enum Languages
    {
        de,
        en
    }


    [Header("Tree Generation Variables")]
    public TreeSetting treeSettings = new TreeSetting();
    [Header("CitySettings")]
    public CitySetting citySettings = new CitySetting();
    [Header("Local Settings")]
    public LocalSettings localSettings = new LocalSettings();

    public static GameObject TreePrefab { get => Instance.treeSettings.treePrefab; set => Instance.treeSettings.treePrefab = value; }
    public static List<GameObject> PlaceableModels { get => Instance.treeSettings.placeableModels; set => Instance.treeSettings.placeableModels = value; }
    public static int MaxPlacedTrees { get => Instance.treeSettings.maxPlacedTrees; set => Instance.treeSettings.maxPlacedTrees = value; }
    public static int MaxForestSize { get => Instance.treeSettings.maxForestSize; set => Instance.treeSettings.maxForestSize = value; }
    public static int MinForestSize { get => Instance.treeSettings.minForestSize; set => Instance.treeSettings.minForestSize = value; }
    public static float SuccessProcent { get => Instance.treeSettings.successProcent; set => Instance.treeSettings.successProcent = value; }
    public static float HeightLimit { get => Instance.treeSettings.heightLimit; set => Instance.treeSettings.heightLimit = value; }
    public static float MinHeight { get => Instance.treeSettings.minHeight; set => Instance.treeSettings.minHeight = value; }
    public static CitySetting CitySettings { get => Instance.citySettings; set => Instance.citySettings = value; }
    public static Languages CurrentLanguage { get => Instance.currentLanguage; set => Instance.currentLanguage = value; }
    public static int CurrLangIndex { get => Instance.currLangIndex; set => Instance.currLangIndex = value; }

    public event Action onResourceTick;
    public event Action OnGameTick;
    public float tickRate;
    public float resourceTickRate;
    public GameObject SelectionPrefab;
    public GameObject CustomPass;

    [HideInInspector]
    public static UIEventManagerAndNotifier UIInstance;


    public void ResourceTicker()
    {
        onResourceTick?.Invoke();
    }
    protected override void Initialize()
    {
        base.Initialize();
        StartCoroutine(TickStarter());
        StartCoroutine(GameTickStarter());
        citySettings.perPlayerSettings.Clear();

        UIInstance = FindObjectOfType<UIEventManagerAndNotifier>();
        if (FindObjectOfType<UIEventManagerAndNotifier>() == null)
        {
            Instantiate(UI_Prefab);
            UIInstance = FindObjectOfType<UIEventManagerAndNotifier>();
        }
        if(GameObject.FindGameObjectWithTag("GlobalVolume") == null)
        {
            DontDestroyOnLoad(Instantiate(CustomPass));
        }
            
    }

    public static void changedLanguage()
    {
        localization.isInit = false;
        localization.Init();
        languageChangeEvent();
    }
    protected override void Deinitialize()
    {
        base.Deinitialize();
        Instance.citySettings.perPlayerSettings.Clear();
    }


    private IEnumerator TickStarter()
    {
        while (true)
        {
            ResourceTicker();

            yield return new WaitForSeconds(resourceTickRate);
        }
    }
    private void GameTick()
    {
        OnGameTick?.Invoke();
    }
    private IEnumerator GameTickStarter()
    {
        while (true)
        {
            GameTick();
            yield return new WaitForSeconds(tickRate);
        }
    }
    [Serializable]
    public class TreeSetting
    {
        public GameObject treePrefab;
        public List<GameObject> placeableModels;
        public int maxPlacedTrees = 200;
        public int maxForestSize = 10;
        public int minForestSize = 2;
        public float heightLimit = 14f;
        public float minHeight = 1.4f;
        [Range(0, 1)]
        public float successProcent = 0.50f;
    }
    [Serializable]
    public class CitySetting
    {

        public List<perPlayerCitySettings> perPlayerSettings = new List<perPlayerCitySettings>();
        public List<Sprite> icons = new List<Sprite>();
        public GameObject cityPrefab;
        public GameObject ResourcePrefab;

        [Serializable]
        public class perPlayerCitySettings
        {
            public int playerID;
            public float exponentialFoodProduction = 2f;
            public float exponentialScienceProduction = 2f;

            public PlayerScript playerScript;

            public float science;
            public float gold;

            public List<citySystem> playerCities;


        }


    }
    public bool enterCityBuildMode(int playerID)
    {
        CitySetting.perPlayerCitySettings ppcs = Instance.citySettings.perPlayerSettings[playerID];
        if (ppcs == null) return false;


        citySystem css = Instantiate(citySettings.cityPrefab).GetComponent<citySystem>();


        ppcs.playerScript.buildObject(css);

        ppcs.playerCities.Add(css);


        return true;
    }
    public bool enterResourceBuildMode(int Resource)
    {
        CitySetting.perPlayerCitySettings ppcs = Instance.citySettings.perPlayerSettings[localPlayerID];
        if (ppcs == null) return false;
        citySystem csr = (citySystem)ppcs.playerScript.Currently_Selected[ppcs.playerScript.Currently_Selected.Count-1];
        if (csr == null) return false;
        ResourceBuildings css = Instantiate(citySettings.ResourcePrefab).GetComponent<ResourceBuildings>();
        css.type = (ResourceTypes)Resource;
        css.resourceCity = csr;
        ppcs.playerScript.buildObject(css);
        
        return true;
    }
    public static void cityBuildmode(int id)
    {
        Instance.enterCityBuildMode(id);
    }
    public static void resourceBuildMode(int resourceID)
    {
        Instance.enterResourceBuildMode(resourceID);
    }
    public static void addPlayer(PlayerScript playerCam)
    {
        CitySetting.perPlayerCitySettings set = new CitySetting.perPlayerCitySettings
        {
            playerID = Instance.citySettings.perPlayerSettings.Count,
            playerScript = playerCam,
            playerCities = new List<citySystem>()
        };
        Instance.citySettings.perPlayerSettings.Add(set);

    }
    public static Sprite GetResourceIcon(ResourceTypes resource)
    {
        return Instance.citySettings.icons[(int)resource];
    }

    public static void ApplySetting()
    {
        Debug.Log(CurrLangIndex);
        CurrentLanguage =(Languages) CurrLangIndex;
        changedLanguage();
    }
}

