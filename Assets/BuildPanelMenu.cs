using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelMenu : MonoBehaviour
{
    public CityInfoPanel CityInfoPanel;
    public simpleUIFader BuildPanelFader;



    public void FadeUI()
    {
        if (BuildPanelFader == null) BuildPanelFader = GetComponent<simpleUIFader>();
        BuildPanelFader.disableObject();
    }
}
