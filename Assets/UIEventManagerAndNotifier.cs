using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    private void Start()
    {
        //StartCoroutine(ie());
    }
    private IEnumerator ie()
    {
        List<GameObject> gc = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            gc.Add(GameObject.Instantiate(side_notification_prefab, strategyModeUI.sideBarNotificationArea.transform));
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
            gc.Add(Instantiate(side_notification_prefab, gc[count - 1].transform.position + new Vector3(0, gc[count - 1].GetComponent<RectTransform>().sizeDelta.y), gc[count - 1].transform.rotation, strategyModeUI.sideBarNotificationArea.transform));
        }
        else
        {
            gc.Add(Instantiate(side_notification_prefab, strategyModeUI.sideBarNotificationArea.position + new Vector3(0, strategyModeUI.sideBarNotificationArea.sizeDelta.y), new Quaternion(), strategyModeUI.sideBarNotificationArea.transform));
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
                item.transform.DOBlendableMoveBy(new Vector3(0, -item.GetComponent<RectTransform>().sizeDelta.y, 0), 0.75f);
                
            }
            iterator++;


        }
        gc.Remove(index);
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(selected);

        switch ((UIEventManagerAndNotifier.type_Of_UI)selected.enumValueIndex)
        {
            case UIEventManagerAndNotifier.type_Of_UI.battleMode:
                EditorGUILayout.PropertyField(bmUI);
                break;
            case UIEventManagerAndNotifier.type_Of_UI.stratMode:
                EditorGUILayout.PropertyField(smUI);
                break;
            case UIEventManagerAndNotifier.type_Of_UI.mainMenu:
                EditorGUILayout.PropertyField(mmUI);
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
    public RectTransform sideBarNotificationArea;
    public RectTransform timedEventArea;
    public RectTransform alertNotificationArea;
    public RectTransform selectionArea;
    public RectTransform topUIBar;
    public RectTransform bottomUIBar;

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


