using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    public AudioMixer masterMixer;

    public static SoundManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SetVolume(float newVolume)
    {
        masterMixer.SetFloat("Volume", newVolume);
    }

    public void MuteVolume()
    {
        masterMixer.SetFloat("Volume", -80f);
    }
}
