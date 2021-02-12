using UnityEngine.Audio;
using System.Collections.Generic;
using yaSingleton;
using UnityEngine;
using FMOD;
using System;

[CreateAssetMenu(fileName = "SFXManager", menuName = "KoWiZe Custom Assets/Singletons/SFXManager")]
public class SFXManagerController : Singleton<SFXManagerController>
{
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SoundEffects;
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Enviroment;
    VolumeTarget target;
    [SerializeField]
    StringSFXDictionary soundeffects = new StringSFXDictionary();
    public IDictionary<string,SFXHolder> AS
    {
        get { return soundeffects; }
        set { soundeffects.CopyFrom(value); }
    }

    [System.Serializable]
    public class SFXHolder
    {
        [FMODUnity.EventRef]
        public string ID;
        [Range(0f, 1f)]
        public float Volume = 0.5f;
        [Range(0f, 5f)]
        public float pitch = 1f;
        [SerializeField]
        public List <FMOD.Studio.EventInstance> instance = new List<FMOD.Studio.EventInstance>();
    }

    protected override void Initialize()
    {
        base.Initialize();
        try
        {
            Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
            SoundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/Sound effects");
            Enviroment = FMODUnity.RuntimeManager.GetBus("bus:/Master/Enviroment");
            Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        }
        finally
        {

        }
        
    }
    public void Play(string name)
    {
        
        SFXHolder s = AS[name];
        FMOD.Studio.EventInstance inst = FMODUnity.RuntimeManager.CreateInstance(s.ID);
        inst.start();
        s.instance.Add(inst);
        
    }

    public void PlayOnObject(string name, GameObject target)
    {
        SFXHolder s = AS[name];
        UnityEngine.Debug.Log(s);
        FMOD.Studio.EventInstance inst = FMODUnity.RuntimeManager.CreateInstance(s.ID);
        inst.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(target.transform.position));
        inst.start();
        inst.setVolume(1f);
        s.instance.Add(inst);

    }
    public void setTarget(string t)
    {
        target = (VolumeTarget)Enum.Parse(typeof(VolumeTarget),t);
    }
    public void VolumeLevel(float newMasterVolume)
    {
        switch (target)
        {
            case VolumeTarget.Master:
                Master.setVolume(newMasterVolume);
                break;
            case VolumeTarget.Enviroment:
                Enviroment.setVolume(newMasterVolume);
                break;
            case VolumeTarget.Music:
                Music.setVolume(newMasterVolume);
                break;
            case VolumeTarget.Effects:
                SoundEffects.setVolume(newMasterVolume);
                break;
            default:
                break;
        }
    }
    public enum VolumeTarget
    {
        Master,
        Enviroment,
        Music,
        Effects
    }


    [System.Serializable]
    public class StringSFXDictionary : SerializableDictionary<string, SFXHolder> { }
}
