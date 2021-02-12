using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using yaSingleton;
using static ResourceClass;
using UnityEngine.UI;
using Mirror;

[CreateAssetMenu(fileName = "GameController", menuName = "KoWiZe Custom Assets/Singletons/GameController")]
public partial class GameController : Singleton<GameController>
{
    private int currLangIndex;
    public Languages currentLanguage = Languages.en;
    public static event Action languageChangeEvent;
    public GameObject UI_Prefab;
    public int localPlayerID = 0;
    public AORNetworkRoomManager manager;
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

    //[HideInInspector]
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
        //Instance.citySettings.perPlayerSettings.Clear();

        UIInstance = FindObjectOfType<UIEventManagerAndNotifier>();
        if (UIInstance == null)
        {
            UIInstance = Instantiate(UI_Prefab).GetComponentInChildren<UIEventManagerAndNotifier>();
        }
        if(GameObject.FindGameObjectWithTag("GlobalVolume") == null)
        {
            DontDestroyOnLoad(Instantiate(CustomPass));
        }
        if(manager == null)
        {
            manager = FindObjectOfType<AORNetworkRoomManager>();
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
    

    
    [ClientRpc]
    public static void AddCityToPlayers(NetworkIdentity city,uint OwnerID)
    {
        Instance.citySettings.perPlayerSettings[GetPlayerIndexbyNetID(OwnerID)].playerCities.Add(city.GetComponent<citySystem>());
    }

    
    
    public static void CmdcityBuildmode()
    {
        Instance.localSettings.localPlayer.CmdenterCityBuildMode();
    }
    public static void resourceBuildMode(int resourceID)
    {
        Instance.localSettings.localPlayer.CmdenterResourceBuildMode(resourceID);
    }
    public static void addPlayer(NetworkIdentity player)
    {
        var playerCam = player.GetComponent<PlayerScript>();
        foreach (var item in Instance.citySettings.perPlayerSettings)
        {
            if (item.playerScript != null &&(item.playerScript.netId == playerCam.netId)) return;
        }
        if (playerCam.isLocalPlayer) Instance.localPlayerID = Instance.citySettings.perPlayerSettings.Count;
        CitySetting.perPlayerCitySettings set = new CitySetting.perPlayerCitySettings
        {
            playerID = (int)playerCam.netId,
            playerScript = playerCam,
            gold = 10,
            science = 0,
            playerCities = new List<citySystem>()
        };
        Instance.citySettings.perPlayerSettings.Add(set);
    }
    public static Sprite GetResourceIcon(ResourceTypes resource)
    {
        return Instance.citySettings.icons[(int)resource];
    }
    public static citySystem TryGetCityFromIDs(uint playerNetID,int cityID)
    {
        foreach (var item in Instance.citySettings.perPlayerSettings[GetPlayerIndexbyNetID(playerNetID)].playerCities)
        {
            if (cityID == item.cityID) return item;
        }
        return null;
    }
    public static int GetPlayerIndexbyNetID(uint netID)
    {
        for (int i = 0; i < Instance.citySettings.perPlayerSettings.Count; i++)
        {
            if (Instance.citySettings.perPlayerSettings[i].playerID == (int)netID) return i;
        }
        return -1;
    }
    public static void ApplySetting()
    {
        Debug.Log(CurrLangIndex);
        CurrentLanguage =(Languages) CurrLangIndex;
        changedLanguage();
    }
}

