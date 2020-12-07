using UnityEngine.Audio;
using System.Collections.Generic;
using yaSingleton;
using UnityEngine;
[CreateAssetMenu(fileName = "SFXManager", menuName = "KoWiZe Custom Assets/Singletons/SFXManager")]
public class SFXManagerController : Singleton<SFXManagerController>
{
    public List<SFXHolder> Holders;
    public Dictionary<mixers, AudioMixer> AS;
    public GameObject Ao;
    public enum mixers
    {
        defaultMixer,
        environment,
        effects,
        voices

    }

    [System.Serializable]
    public class SFXHolder
    {
        public string ID;
        public AudioClip Audio;
        public mixers mixerToUse = mixers.defaultMixer;
        [Range(0f, 1f)]
        public float Volume = 0.5f;
        [Range(0f, 5f)]
        public float pitch = 1f;

        [HideInInspector]
        public AudioSource source;
    }


    protected override void Initialize()
    {
        base.Initialize();
        Ao = new GameObject();
        DontDestroyOnLoad(Ao);
        foreach (SFXHolder s in Holders)
        {
            s.source = Ao.AddComponent<AudioSource>();
            s.source.clip = s.Audio;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
        }

    }

    public void Play(string name)
    {
        SFXHolder s = Holders.Find(sound => sound.ID == name);
        s.source.Play();
    }

    public void PlayOnObject(string name, GameObject target)
    {
        SFXHolder s = Holders.Find(sound => sound.ID == name);
        AudioSource sd =target.AddComponent<AudioSource>();
        sd = s.source;
        s.source = sd;
        s.source.Play();
    }
}
