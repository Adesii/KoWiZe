using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class AORTechTreeItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI Techname;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Image progressBar;
    public GameObject BlackOut;
    public GameObject LightOut;

    public TechNode ownNode;
    public RectTransform INput;
    public RectTransform OUTput;

    public LineGraphic ownGraphicsrender;

    public Color inactive;
    public Color active;
    public Color unlocked;
    public float fadeTime= 0.2f;

    private void Start()
    {
        if (ownNode.isUnlocked) TechTreeManager.Instance.unlockedTech(ownNode);
        ownNode.onUnlocked.AddListener(onUnlocked);
        ownNode.onAvailable += updateNodes;
        updateNodes();
    }

    public void updateNodes()
    {
        if (!ownNode.isUnlocked && !ownNode.isAvailable) ownGraphicsrender.DOColor(inactive, fadeTime);
        else if(!ownNode.isUnlocked && ownNode.isAvailable) ownGraphicsrender.DOColor(active, fadeTime);
        else ownGraphicsrender.DOColor(unlocked, fadeTime);
        BlackOut.SetActive(!ownNode.isAvailable);
        LightOut.SetActive(ownNode.isUnlocked);
    }
    void onUnlocked(TechNode n)
    {

        updateNodes();
    }
    public void newNode()
    {
        Techname.text = ownNode.TechName;
        description.text = ownNode.TechDescription;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ownNode.isUnlocked = true;
    }
}
