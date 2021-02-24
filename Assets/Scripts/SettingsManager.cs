using System;
using System.Collections.Generic;
using UnityEngine;
using SharpConfig;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static Action onSave;
    public static Action onLoad;
    public static Configuration settings = new Configuration();
    public static string CfgPath { get => (Application.persistentDataPath + "/cfg"); }

    public static SettingsManager instance { get; set; }

    [SerializeField]
    private HDAdditionalCameraData _cameraData;
    private FrameSettings _frameSettings;
    private FrameSettingsOverrideMask _frameSettingsOverrideMask;
    List<Setting> allSettings;

    public static void SaveSettings()
    {
        print($"Saved Settings To Path {CfgPath}");
        onSave?.Invoke();
        if (!Directory.Exists(CfgPath)) Directory.CreateDirectory(CfgPath);
        settings.SaveToFile(CfgPath + "/game.cfg");
        init();
    }
    private void Awake()
    {
        instance = this;
        LoadSettings();
        initSettings();
        applyAllSettings();

    }
    private void Start()
    {
        //onLoad?.Invoke();
        GameController.ApplySetting();
    }
    public void LoadSettings()
    {

        if (!Directory.Exists(CfgPath)) Directory.CreateDirectory(CfgPath);
        //Debug.Log(Resources.Load<TextAsset>("game"));
        if (!File.Exists(CfgPath + "/game.cfg")) settings = Configuration.LoadFromString(Resources.Load<TextAsset>("game").text);
        else
        settings = Configuration.LoadFromFile(CfgPath + "/game.cfg");
        print($"Loaded Settings From Path {CfgPath}");
    }

    private void applyAllSettings()
    {

        allSettings = FindObjectsOfType<Setting>(true).ToList();
        foreach (var item in allSettings)
        {
            var val = settings[item.category][item.settingID.GetComponent<TextMeshProUGUI>().text];
            print(val);
            switch (item.SettingType)
            {
                case Setting.SettingValueType.Float:
                    //item.Holder.GetComponent<Slider>().value = val.FloatValue;
                    SFXManagerController.Instance.setTarget(item.settingID.GetComponent<TextMeshProUGUI>().text);
                    SFXManagerController.Instance.VolumeLevel(val.FloatValue);
                    break;
                default:
                    break;
            }
        }
    }

    public static void init()
    {
        instance.initSettings();
    }
    public void initSettings()
    {
        if (settings["Shadows"]["MMS_SoftShadows"].BoolValue)
            QualitySettings.shadows = ShadowQuality.All;
        else QualitySettings.shadows = ShadowQuality.HardOnly;
        if (((ShadowResolution)settings["Shadows"]["MMS_ShadowQuality"].IntValue) == ShadowResolution.Low)
            QualitySettings.shadows = ShadowQuality.Disable;
        else
            QualitySettings.shadowResolution = (ShadowResolution)settings["Shadows"]["MMS_ShadowQuality"].IntValue;


        switch (settings["Shadows"]["MMS_Distance"].IntValue)
        {
            case 0:
                QualitySettings.shadowCascades = 1;
                break;
            case 1:
                QualitySettings.shadowCascades = 2;
                break;
            case 2:
                QualitySettings.shadowCascades = 4;
                break;
            default:
                break;
        }
        switch (settings["Model"]["MMS_LODDistance"].IntValue)
        {
            case 0:
                QualitySettings.lodBias = 0.5f;
                break;
            case 1:
                QualitySettings.lodBias = 1;
                break;
            case 2:
                QualitySettings.lodBias = 2;
                break;
            default:
                break;
        }

        switch (settings["Model"]["MMS_TextureQuality"].IntValue)
        {
            case 0:
                QualitySettings.masterTextureLimit = 2;
                break;
            case 1:
                QualitySettings.masterTextureLimit = 1;

                break;
            case 2:
                QualitySettings.masterTextureLimit = 0;
                break;
            default:
                break;
        }
        GameController.CurrLangIndex = settings["Language"]["MMS_Lang"].IntValue;
    }
}