using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


[Serializable]
public class UIEventManagerAndNotifier : MonoBehaviour
{

    [Header("Notification Prefabs")]

    public PrefabArray[] prefabArray = new PrefabArray[5];
    [Serializable]
    public struct PrefabArray
    {
        public List<GameObject> prefabs;
    }


    [Header("Type Selection")]
    public type_Of_UI selectedUI;
    [SerializeField]
    public stratModeUI strategyModeUI = new stratModeUI();
    [SerializeField]
    public menuUI menuUI = new menuUI();

    [SerializeField]
    public battleModeUI battleModeUI = new battleModeUI();


    [HideInInspector]
    public static List<GameObject> gc = new List<GameObject>();

    public static Canvas can;
    private void Start()
    {
        //StartCoroutine(ie());
        can = gameObject.GetComponent<Canvas>();
        DontDestroyOnLoad(gameObject);
        updateUISelection();


    }

    private void updateUISelection()
    {
        strategyModeUI.UIItem.SetActive(selectedUI == type_Of_UI.stratMode);
        menuUI.UIItem.SetActive(selectedUI == type_Of_UI.mainMenu);

    }
    private IEnumerator ie()
    {
        List<GameObject> gc = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            gc.Add(GameObject.Instantiate(prefabArray[(int)NotificationType.SideBar].prefabs[0], strategyModeUI.sideBarNotificationArea.transform));
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);
        foreach (var item in gc)
        {
            item.transform.GetChild(0).GetComponent<sideBarNotification>().kill();
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForEndOfFrame();
    }

    public void notify()
    {
        int count = gc.Count();

        if (count > 0)
        {
            gc.Add(Instantiate(prefabArray[(int)NotificationType.SideBar].prefabs[0], gc[count - 1].GetComponent<RectTransform>().position + new Vector3(0, getSizeY(gc[count - 1].GetComponent<RectTransform>())), gc[count - 1].transform.rotation, strategyModeUI.sideBarNotificationArea.transform));
        }
        else
        {
            gc.Add(Instantiate(prefabArray[(int)NotificationType.SideBar].prefabs[0], strategyModeUI.sideBarNotificationArea.position - new Vector3(0, getSizeY(strategyModeUI.sideBarNotificationArea)), new Quaternion(), strategyModeUI.sideBarNotificationArea.transform));
        }
    }
    public static void moveDown(GameObject index)
    {
        int i = gc.IndexOf(index);
        int iterator = 0;
        foreach (var item in gc.ToList())
        {
            if (item != null && iterator >= i && item != index)
            {
                //Vector3 cs = gc[iterator - 1].transform.position;
                //item.transform.DOMove(cs,1f);
                item.transform.DOBlendableMoveBy(new Vector3(0, -getSizeY(item.GetComponent<RectTransform>()), 0), 0.75f);

            }
            iterator++;


        }
        gc.Remove(index);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void NewGame()
    {
        
        if (menuUI.BlackScreen != null)
        {
            menuUI.BlackScreen.SetActive(true);
        }

        selectedUI = type_Of_UI.stratMode;

        InvokeRepeating(nameof(LoadnewScene), 1, 0);
        InvokeRepeating(nameof(FadeScene), 1.5f, 0);

    }

    private void LoadnewScene()
    {
        SceneManager.LoadScene(1);
        GameController.Instance.manager.StartHost();
    }

    private void FadeScene()
    {
        updateUISelection();
        menuUI.BlackScreen.GetComponent<simpleUIFader>().disableObject();
    }

    public static float getSizeY(RectTransform rt)
    {
        //return rt.sizeDelta.y;
        return RectTransformUtility.PixelAdjustRect(rt, can).height;
    }
    public enum type_Of_UI
    {
        battleMode,
        stratMode,
        mainMenu
    }

    public enum NotificationType
    {
        Alert,
        SideBar,
        Event,
        TimedEvent,
        Countdown
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(UIEventManagerAndNotifier))]
[CanEditMultipleObjects]
public class drawCustomWindowUI : Editor
{
    protected SerializedProperty currentPorperty;
    protected SerializedProperty currentSelection;
    Vector2 scrollPosition;

    private void OnEnable()
    {
        currentSelection = serializedObject.FindProperty("selectedUI");
    }
    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();
                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                if (p.GetType() == typeof(GameObject))
                {
                    EditorGUILayout.ObjectField(p);
                }
                else
                {
                    EditorGUILayout.PropertyField(p, drawChildren);

                }
            }

        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel++;
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition, GUILayout.Height(200));
        EditorGUILayout.BeginVertical();
        int Count = 0;
        foreach (SerializedProperty item in serializedObject.FindProperty("prefabArray"))
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(5));
            EditorGUILayout.LabelField(((UIEventManagerAndNotifier.NotificationType)Count).ToString());
            EditorGUILayout.PropertyField(item.FindPropertyRelative("prefabs"));
            EditorGUILayout.EndHorizontal();
            Count++;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(currentSelection);
        EditorGUI.indentLevel++;
        switch ((UIEventManagerAndNotifier.type_Of_UI)currentSelection.intValue)
        {
            case UIEventManagerAndNotifier.type_Of_UI.battleMode:
                currentPorperty = serializedObject.FindProperty("battleModeUI");
                DrawProperties(currentPorperty, true);
                break;
            case UIEventManagerAndNotifier.type_Of_UI.stratMode:
                currentPorperty = serializedObject.FindProperty("strategyModeUI");
                DrawProperties(currentPorperty, true);
                break;
            case UIEventManagerAndNotifier.type_Of_UI.mainMenu:
                currentPorperty = serializedObject.FindProperty("menuUI");
                DrawProperties(currentPorperty, true);
                break;
            default:
                break;
        }
        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();


    }
}
#endif

[Serializable]
public class stratModeUI
{
    public GameObject UIItem;
    [Header("UI Areas")]
    public RectTransform sideBarNotificationArea;
    public RectTransform timedEventArea;
    public RectTransform alertNotificationArea;
    public RectTransform selectionArea;
    public RectTransform topUIBar;
    public RectTransform bottomUIBar;

    public GameObject BuildPanel;
    private BuildPanelMenu buildPanelScript;
    private CityInfoPanel city;
    public CityInfoPanel City
    {
        get
        {
            if (city != null) return BuildPanelScript.CityInfoPanel;
            else
            {
                city = buildPanelScript.CityInfoPanel;
                return city;
            }
        }
    }

    public BuildPanelMenu BuildPanelScript
    {
        get
        {
            if (buildPanelScript == null)
            {
                buildPanelScript = BuildPanel.GetComponent<BuildPanelMenu>();

            }
            return buildPanelScript;
        }
    }
}
[Serializable]
public class battleModeUI
{
    public GameObject UIItem;

    public string ss = "working";
}
[Serializable]

public class menuUI
{
    public GameObject UIItem;

    public GameObject BlackScreen;
    public string ss = "working";
}