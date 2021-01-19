using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SFXManagerController.Instance.Play("bgm_menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
