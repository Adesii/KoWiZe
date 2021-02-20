using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalisationUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    private string unString;
    public string unlocalizedString;
    public void Start()
    {
        newUnlocalisationChange();
        GameController.languageChangeEvent += changeLangue;
    }
    public void changeLangue()
    {
        string temp = localization.GetLocalisedValue(unlocalizedString);
        if (!string.IsNullOrEmpty(temp))
            textField.text = temp;
    }

    public void newUnlocalisationChange()
    {
        if (textField == null) textField = GetComponent<TextMeshProUGUI>();
        unlocalizedString = textField.text;
        changeLangue();
    }
}
