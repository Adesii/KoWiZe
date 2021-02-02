using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RuntimeManager.StudioSystem.setParameterByName("GlobalState", 3f);
        SFXManagerController.Instance.Play("bgm_menu");
    }
    private void OnDestroy()
    {
        RuntimeManager.StudioSystem.setParameterByName("GlobalState", 0f);

    }
}
