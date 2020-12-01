using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalisationUI : MonoBehaviour
{
    TextMeshProUGUI textField;
    public string key;


    // Start is called before the first frame update
    void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.text = localization.GetLocalisedValue(key);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
