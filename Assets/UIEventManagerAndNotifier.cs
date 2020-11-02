using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class UIEventManagerAndNotifier : MonoBehaviour
{
    [Header("Notification Prefabs")]
    public GameObject side_notification_prefab;

    [HideInInspector]
    [Header("Type Selection")]
    public type_Of_UI selectedUI;
    [HideInInspector]
    public stratModeUI strategyModeUI = new stratModeUI();
    [HideInInspector]
    public menuUI menuUI = new menuUI();
    [HideInInspector]
    public battleModeUI battleModeUI = new battleModeUI();

    [HideInInspector]
    public static List<GameObject> gc = new List<GameObject>();

    public static Canvas can;
    private void Start()
    {
        //StartCoroutine(ie());
        can = gameObject.GetComponent<Canvas>();
    }
    private IEnumerator ie()
    {
        List<GameObject> gc = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            gc.Add(GameObject.Instantiate(side_notification_prefab, strategyModeUI.SideBarNotificationArea.transform));
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
            gc.Add(Instantiate(side_notification_prefab, gc[count - 1].GetComponent<RectTransform>().position + new Vector3(0, getSizeY(gc[count - 1].GetComponent<RectTransform>())), gc[count - 1].transform.rotation, strategyModeUI.SideBarNotificationArea.transform));
        }
        else
        {
            gc.Add(Instantiate(side_notification_prefab, strategyModeUI.SideBarNotificationArea.position - new Vector3(0, getSizeY(strategyModeUI.SideBarNotificationArea)), new Quaternion(), strategyModeUI.SideBarNotificationArea.transform));
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

[CustomEditor(typeof(UIEventManagerAndNotifier))]
[CanEditMultipleObjects]
public class UICLassEditor : Editor
{
    SerializedProperty selected;
    SerializedProperty smUI;
    SerializedProperty bmUI;
    SerializedProperty mmUI;

    private void OnEnable()
    {
        selected = serializedObject.FindProperty("selectedUI");
        smUI = serializedObject.FindProperty("strategyModeUI");
        bmUI = serializedObject.FindProperty("battleModeUI");
        mmUI = serializedObject.FindProperty("menuUI");
    }
    private void show(SerializedProperty objects)
    {
        foreach (var item in objects.GetType().GetProperties())
        {
            EditorGUILayout.PropertyField(item);
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(selected);

        switch ((UIEventManagerAndNotifier.type_Of_UI)selected.enumValueIndex)
        {
            case UIEventManagerAndNotifier.type_Of_UI.battleMode:
                //EditorGUILayout.PropertyField(bmUI, true);
                //CreateEditor(bmUI.objectReferenceValue).OnInspectorGUI();
                show(bmUI);

                break;
            case UIEventManagerAndNotifier.type_Of_UI.stratMode:
                EditorGUILayout.PropertyField(smUI, true);

                break;
            case UIEventManagerAndNotifier.type_Of_UI.mainMenu:
                EditorGUILayout.PropertyField(mmUI, true);
                CreateEditor(mmUI.serializedObject.targetObject).OnInspectorGUI();
                break;
            default:
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

}
[Serializable]
public class stratModeUI
{
    [Header("UI Areas")]
    private RectTransform sideBarNotificationArea;
    private RectTransform timedEventArea;
    private RectTransform alertNotificationArea;
    private RectTransform selectionArea;
    private RectTransform topUIBar;
    private RectTransform bottomUIBar;

    public RectTransform SideBarNotificationArea { get => sideBarNotificationArea; set => sideBarNotificationArea = value; }
    public RectTransform TimedEventArea { get => timedEventArea; set => timedEventArea = value; }
    public RectTransform AlertNotificationArea { get => alertNotificationArea; set => alertNotificationArea = value; }
    public RectTransform SelectionArea { get => selectionArea; set => selectionArea = value; }
    public RectTransform TopUIBar { get => topUIBar; set => topUIBar = value; }
    public RectTransform BottomUIBar { get => bottomUIBar; set => bottomUIBar = value; }
}
[Serializable]

public class battleModeUI
{
    public string ss = "working";
}
[Serializable]

public class menuUI
{
    public string ss = "working";
}


