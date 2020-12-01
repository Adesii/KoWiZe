using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVLoader
{
    private static TextAsset csvFile;
    private char lineSeperator = '\n';
    private char surround = '"';
    private string[] fieldSeperator = { "\",\"" };

    public static CSVLoader instance;
    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("locale/"+GameController.CurrentLanguage.ToString()+"_"+GameController.CurrentLanguage.ToString().ToUpper());
        Debug.Log(csvFile);
    }
    public Dictionary<string, string> GetDictionaryValues(string attributeID)
    {

        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] lines = csvFile.text.Split(lineSeperator);
        int attributeIndex = -1;

        string[] headers = lines[0].Split(fieldSeperator, StringSplitOptions.None);
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attributeID))
            {
                attributeIndex = i;
                break;
            }
        }
        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = CSVParser.Split(line);
            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd(surround);
            }
            if (fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) { continue; }
                if (attributeIndex == -1) continue;
                string value = fields[attributeIndex];
                dictionary.Add(key, value);
            }
        }
        return dictionary;
    }
    public Dictionary<string, string> GetDictionaryFromFile(TextAsset csvFile)
    {

        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] lines = csvFile.text.Split(lineSeperator);

        string[] headers = lines[0].Split(fieldSeperator, StringSplitOptions.None);
        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = CSVParser.Split(line);
            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd(surround);

            }
            dictionary.Add(fields[0], fields[1]);
        }
        return dictionary;
    }
}
