using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AORUnitAttrShow : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI attrName;
    [SerializeField]
    TextMeshProUGUI attribute;

    public void SetAttr(string name,string attr)
    {
        attrName.text = name;
        attribute.text = attr;
    }
}
