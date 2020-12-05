using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public float FadeTime;
    public TabButton selectedTab;
    public Vector3 scaler;

    public List<GameObject> swappingItems;
    bool firstSelect = false;
    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        else
        {
            if(selectedTab != null && tabButtons.Contains(selectedTab) &&!firstSelect)
            {
                selectedTab.transform.DOBlendableScaleBy(scaler, FadeTime);
                selectedTab.transform.GetChild(0).DOBlendableScaleBy(-scaler, FadeTime);
                OnTabSelected(selectedTab);
                firstSelect = true;
            }
        }
        tabButtons.Add(button);
        ResetTabs();
    }
    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.DOBlendableColor(tabHover, FadeTime);
            button.transform.DOBlendableScaleBy(scaler, FadeTime);
            button.transform.GetChild(0).DOBlendableScaleBy(-scaler, FadeTime);
        }
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();

        if (selectedTab == null || button != selectedTab)
        {
            button.transform.DOBlendableScaleBy(-scaler, FadeTime);
            button.transform.GetChild(0).DOBlendableScaleBy(scaler, FadeTime);

        }

    }
    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.DOBlendableColor(tabActive,FadeTime);

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < swappingItems.Count; i++)
        {
            if(i == index)
            {
                swappingItems[i].SetActive(true);
            }
            else
            {
                swappingItems[i].SetActive(false);
            }
        }
    }


    public void ResetTabs()
    {
        foreach (TabButton item in tabButtons)
        {
            if(selectedTab != null && item == selectedTab) { continue; }
            item.background.color = tabIdle;
            if(item.transform.localScale.y >1 && !DOTween.IsTweening(item.transform))
            {
                item.transform.GetChild(0).DOScaleY(1, FadeTime);
                item.transform.DOScaleY(1, FadeTime);
            }
        }
    }

}
