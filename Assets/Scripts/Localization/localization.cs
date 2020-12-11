using System.Collections.Generic;
using UnityEngine;

public class localization : MonoBehaviour
{

    private static Dictionary<string, string> localised;
    public static bool isInit;

    public static void Init()
    {

        if(CSVLoader.instance == null)
        {
            CSVLoader.instance = new CSVLoader();
        }
        CSVLoader.instance.LoadCSV();

        localised = CSVLoader.instance.GetDictionaryValues(GameController.CurrentLanguage.ToString());

        isInit = true;
    }
    public static string GetLocalisedValue(string key)
    {
        if (!isInit) { Init(); }
        string value = key;
        if (value != null)
        localised.TryGetValue(key, out value);
        return value;
    }
}
