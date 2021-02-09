using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SoundEffects;
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Enviroment;
    float MusicVolume = 0.5f;
    float SoundEffectsVolume = 1f;
    float MasterVolume = 1f;
    float EnviromentVolume = 1f;
 
    void Awake()
    {
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        SoundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/Sound effects");
        SoundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/Enviroment");
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        
    }

   
    void Update()
    {
        Music.setVolume(MusicVolume);
        SoundEffects.setVolume(SoundEffectsVolume);
        Master.setVolume(MasterVolume);
        Enviroment.setVolume(EnviromentVolume);
    }

    public void MasterVolumeLevel (float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }

    public void SoundEffectsVolumeLevel(float newSoundEffectsVolume)
    {
        SoundEffectsVolume = newSoundEffectsVolume;
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }

    public void EnviromentVolumeLevel(float newEnviromentVolume)
    {
        EnviromentVolume = newEnviromentVolume;
    }
}
