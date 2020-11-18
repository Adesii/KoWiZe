using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance { get { return _instance; } }

    [Header("Tree Generation Variables")]
    public TreeSetting treeSettings = new TreeSetting();
    [Header("CitySettings")]
    public CitySetting citySettings = new CitySetting();

    public static GameObject TreePrefab { get => _instance.treeSettings.treePrefab; set => _instance.treeSettings.treePrefab = value; }
    public static List<GameObject> PlaceableModels { get => _instance.treeSettings.placeableModels; set => _instance.treeSettings.placeableModels = value; }
    public static int MaxPlacedTrees { get => _instance.treeSettings.maxPlacedTrees; set => _instance.treeSettings.maxPlacedTrees = value; }
    public static int MaxForestSize { get => _instance.treeSettings.maxForestSize; set => _instance.treeSettings.maxForestSize = value; }
    public static int MinForestSize { get => _instance.treeSettings.minForestSize; set => _instance.treeSettings.minForestSize = value; }
    public static float SuccessProcent { get => _instance.treeSettings.successProcent; set => _instance.treeSettings.successProcent = value; }
    public static float HeightLimit { get => _instance.treeSettings.heightLimit; set => _instance.treeSettings.heightLimit = value; }
    public static float MinHeight { get => _instance.treeSettings.minHeight; set => _instance.treeSettings.minHeight = value; }
    public static CitySetting CitySettings { get => _instance.citySettings; set => _instance.citySettings = value; }
    public event Action onResourceTick;
    public float tickRate;


    public void ResourceTicker()
    {
        if(onResourceTick != null)
        {
            onResourceTick(); 
        }
    }
    private void Start()
    {
        StartCoroutine(TickStarter());
    }
    private IEnumerator TickStarter()
    {
        while (true)
        {
            ResourceTicker();

            yield return new WaitForSeconds(tickRate);
        } 
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
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

        [Serializable]
        public class perPlayerCitySettings
        {
            public int playerID;
            public float exponentialFoodProduction = 2f;
            public float exponentialScienceProduction = 2f;

            public float science;
            public float gold;

            public List<citySystem> playerCities;


        }
    }
}

