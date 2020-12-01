using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class localization : MonoBehaviour
{
    public enum Language
    {
        English
    }
    public static Language language = Language.English;

    private static Dictionary<string, string> localisedEN;
    public static bool isInit;

    public static void Init()
    {
        CSVLoader csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localisedEN = csvLoader.GetDictionaryValues("en");

        isInit = true;
    }
    public static string GetLocalisedValue(string key)
    {
        if (!isInit) { Init(); }
        string value = key;
        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
        }
        return value;
    }
}
