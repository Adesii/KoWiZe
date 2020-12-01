using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem current;

    public toolTip ToolTip;

    private void Awake()
    {
        current = this;
    }


    public static void Show(string content,string Header = "")
    {
        current.ToolTip.SetText(content, Header);
        current.ToolTip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.ToolTip.gameObject.SetActive(false);
    }


}
