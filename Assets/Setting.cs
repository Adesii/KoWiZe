using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class Setting : MonoBehaviour
{
    [SerializeField]
    private TextLocalisationUI settingID;
    [SerializeField]
    private string category = "General";
    private SharpConfig.Setting SharpSetting;
    [SerializeField]
    private SettingValueType SettingType = SettingValueType.String;
    [SerializeField]
    private GameObject Holder;

    public string SettingKey;

    public void ValueChanged(bool BoolValue)
    {
        SaveSetting(BoolValue);
    }

    public void ValueChanged(float FloatValue)
    {
        SaveSetting(FloatValue);
    }
    public void ValueChanged(int IntValue)
    {
        SaveSetting(IntValue);
    }
    public void ValueChanged(string StringValue)
    {
        SaveSetting(StringValue);
    }
    public void SaveSetting(object value)
    {
        if (settingID.unlocalizedString == null) return;
        SharpSetting = new SharpConfig.Setting(settingID.unlocalizedString, value);
        if (SettingsManager.settings[category][settingID.unlocalizedString] != null)
        {
            SettingsManager.settings[category].Remove(settingID.unlocalizedString);

        }
        SettingsManager.settings[category].Add(SharpSetting);

    }
    private void Awake()
    {
        SettingsManager.onLoad += Loaded;
        SettingsManager.onSave += Saved;
    }
    private void Start()
    {
        Loaded();
    }

    public void Saved()
    {
        if (!SettingsManager.settings[category].Contains(settingID.unlocalizedString))
        {
            print($"no Setting found for {settingID.unlocalizedString}");
            Holder.GetComponent<IAORSettingsSaver>().TriggerUpdate();
        }
    }
    public void Loaded()
    {
        
        
        var val = SettingsManager.settings[category][settingID.unlocalizedString];
        print(val);
        switch (SettingType)
        {
            case SettingValueType.String:
                break;
            case SettingValueType.Float:
                Holder.GetComponent<Slider>().value = val.FloatValue;
                break;
            case SettingValueType.Int:
                Holder.GetComponent<TMP_Dropdown>().value = val.IntValue;
                break;
            case SettingValueType.Bool:
                Holder.GetComponent<Toggle>().isOn = val.BoolValue;
                break;
            default:
                break;
        }

    }

    public enum SettingValueType
    {
        String,
        Float,
        Int,
        Bool
    }
}
