using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalisationUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    private string unString;
    public string unlocalizedString;
    // Start is called before the first frame update
    private void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        unlocalizedString = textField.text;
    }
    void Start()
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
        unlocalizedString = textField.text;
        changeLangue();
    }
}
