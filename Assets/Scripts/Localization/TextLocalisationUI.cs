using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalisationUI : MonoBehaviour
{
    TextMeshProUGUI textField;
    string unlocalizedString;
    // Start is called before the first frame update
    void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        unlocalizedString = textField.text;
        string temp = localization.GetLocalisedValue(unlocalizedString);
        if (!string.IsNullOrEmpty(temp))
            textField.text = temp;
        GameController.languageChangeEvent += changeLangue;
    }
    private void OnEnable()
    {
        changeLangue();
    }
    public void changeLangue()
    {
        string temp = localization.GetLocalisedValue(unlocalizedString);
        if (!string.IsNullOrEmpty(temp))
            textField.text = temp;
    }

    public void newUnlocalisationChange()
    {
        unlocalizedString = textField.text;
        changeLangue();
    }
}
