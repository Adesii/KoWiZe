using TMPro;
using UnityEngine;

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
        textField.text = localization.GetLocalisedValue(textField.text);
        GameController.languageChangeEvent += changeLangue;
    }

    private void changeLangue()
    {
        textField.text = localization.GetLocalisedValue(unlocalizedString);
    }
}
