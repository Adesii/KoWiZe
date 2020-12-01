using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using yaSingleton;

[CreateAssetMenu(fileName = "GameController", menuName = "KoWiZe Custom Assets/Singletons/GameController")]
public class GameController : Singleton<GameController>
{

    [Header("Tree Generation Variables")]
    public TreeSetting treeSettings = new TreeSetting();
    [Header("CitySettings")]
    public CitySetting citySettings = new CitySetting();

    public static GameObject TreePrefab { get => Instance.treeSettings.treePrefab; set => Instance.treeSettings.treePrefab = value; }
    public static List<GameObject> PlaceableModels { get => Instance.treeSettings.placeableModels; set => Instance.treeSettings.placeableModels = value; }
    public static int MaxPlacedTrees { get => Instance.treeSettings.maxPlacedTrees; set => Instance.treeSettings.maxPlacedTrees = value; }
    public static int MaxForestSize { get => Instance.treeSettings.maxForestSize; set => Instance.treeSettings.maxForestSize = value; }
    public static int MinForestSize { get => Instance.treeSettings.minForestSize; set => Instance.treeSettings.minForestSize = value; }
    public static float SuccessProcent { get => Instance.treeSettings.successProcent; set => Instance.treeSettings.successProcent = value; }
    public static float HeightLimit { get => Instance.treeSettings.heightLimit; set => Instance.treeSettings.heightLimit = value; }
    public static float MinHeight { get => Instance.treeSettings.minHeight; set => Instance.treeSettings.minHeight = value; }
    public static CitySetting CitySettings { get => Instance.citySettings; set => Instance.citySettings = value; }
    public event Action onResourceTick;
    public float tickRate;


    public void ResourceTicker()
    {
        if (onResourceTick != null)
        {
            onResourceTick();
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
        StartCoroutine(TickStarter());
        citySettings.perPlayerSettings.Clear();
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

        public GameObject cityPrefab;

        [Serializable]
        public class perPlayerCitySettings
        {
            public int playerID;
            public float exponentialFoodProduction = 2f;
            public float exponentialScienceProduction = 2f;

            public planeCamera playerScript;

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
    public static void cityBuildmode(int id)
    {
        Instance.enterCityBuildMode(id);
    }
    public static void addPlayer(planeCamera playerCam)
    {
        CitySetting.perPlayerCitySettings set = new CitySetting.perPlayerCitySettings
        {
            playerID = Instance.citySettings.perPlayerSettings.Count,
            playerScript = playerCam,
            playerCities = new List<citySystem>()
        };
        Instance.citySettings.perPlayerSettings.Add(set);

    }
}

