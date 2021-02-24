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
    [SerializeField]
    private TextMeshProUGUI progress;
    [SerializeField]
    private GameObject currRess;
    public GameObject BlackOut;
    public GameObject LightOut;

    public TechNode ownNode;
    public RectTransform INput;
    public RectTransform OUTput;

    public AORTechTreeMenuManager ownManager;

    public LineGraphic ownGraphicsrender;

    public Color inactive;
    public Color active;
    public Color unlocked;
    public float fadeTime = 0.2f;

    float currProgress = 0f;

    public bool currResearching = false;
    float lastProgress = 0f;

    private void Start()
    {
        if (ownNode.isUnlocked)
        {
            TechTreeManager.Instance.unlockedTech(ownNode);
            currProgress = ownNode.TechnologyCost;
            progressBar.fillAmount = 1f;
            progress.text = $"{ownNode.TechnologyCost}/{ownNode.TechnologyCost}";

        }
        ownNode.onUnlocked.AddListener(onUnlocked);
        ownNode.onAvailable += updateNodes;
        updateNodes();

        GameController.Instance.onResourceTick += technologyResearch;
    }

    private void technologyResearch()
    {
        if (!currResearching) return;
        var n = GameController.Instance.localSettings.GainAmount.Find((e) => e.Resource == ResourceClass.ResourceTypes.Science).amount;
        if (currProgress + n >= ownNode.TechnologyCost)
        {
            currProgress = ownNode.TechnologyCost;
            ownNode.isUnlocked = true;
        }
        else
        {
            currProgress += n;
        }
    }

    public void updateNodes()
    {
        if (!ownNode.isUnlocked && !ownNode.isAvailable) ownGraphicsrender.DOColor(inactive, fadeTime);
        else if (!ownNode.isUnlocked && ownNode.isAvailable) ownGraphicsrender.DOColor(active, fadeTime);
        else ownGraphicsrender.DOColor(unlocked, fadeTime);
        BlackOut.SetActive(!ownNode.isAvailable);
        LightOut.SetActive(ownNode.isUnlocked);
    }
    private void Update()
    {
        if (currResearching)
        {
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, currProgress / ownNode.TechnologyCost, Time.deltaTime * 4f);

            lastProgress = Mathf.Lerp(lastProgress, currProgress, Time.deltaTime * 2f);
            progress.text = $"{Mathf.CeilToInt(lastProgress)}/{ownNode.TechnologyCost}";

        }
        else if(!currResearching && currRess.activeSelf) currRess.SetActive(false);

    }
    void onUnlocked(TechNode n)
    {
        updateNodes();
        currResearching = false;
        currProgress = ownNode.TechnologyCost;
        progressBar.fillAmount = 1f;
        progress.text = $"{ownNode.TechnologyCost}/{ownNode.TechnologyCost}";
    }
    public void newNode()
    {
        Techname.text = ownNode.TechName;
        description.text = ownNode.TechDescription;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!currResearching && ownNode.isAvailable && !ownNode.isUnlocked)
        {
            currResearching = true;
            ownManager.startResearch(ownNode);
            currRess.SetActive(true);
        }

    }
}
