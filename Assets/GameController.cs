using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance { get { return _instance; } }

    [Header("Tree Generation Variables")]
    public GameObject treePrefab;
    public List<GameObject> placeableModels;
    public int maxPlacedTrees = 200;
    public int maxForestSize = 10;
    public int minForestSize = 2;
    public float heightLimit = 14f;
    public float minHeight = 1.4f;
    [Range(0, 1)]
    public float successProcent = 0.50f;

    public static GameObject TreePrefab { get => _instance.treePrefab; set => _instance.treePrefab = value; }
    public static List<GameObject> PlaceableModels { get => _instance.placeableModels; set => _instance.placeableModels = value; }
    public static int MaxPlacedTrees { get => _instance.maxPlacedTrees; set => _instance.maxPlacedTrees = value; }
    public static int MaxForestSize { get => _instance.maxForestSize; set => _instance.maxForestSize = value; }
    public static int MinForestSize { get => _instance.minForestSize; set => _instance.minForestSize = value; }
    public static float SuccessProcent { get => _instance.successProcent; set => _instance.successProcent = value; }
    public static float HeightLimit { get => _instance.heightLimit; set => _instance.heightLimit = value; }
    public static float MinHeight { get => _instance.minHeight; set => _instance.minHeight = value; }




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

}
