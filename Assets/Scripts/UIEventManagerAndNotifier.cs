﻿using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
        SceneManager.LoadScene(1);
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
public class drawCustomWindowUI : Editor
{
    protected SerializedProperty currentPorperty;
    protected SerializedProperty currentSelection;
    Vector2 scrollPosition;

    private void OnEnable()
    {
        currentSelection = serializedObject.FindProperty("selectedUI");
    }
    protected void DrawProperties(SerializedProperty prop,bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if(p.isArray && p.propertyType == SerializedPropertyType.Generic)
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
                if(!string.IsNullOrEmpty(lastPropPath)&& p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
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





