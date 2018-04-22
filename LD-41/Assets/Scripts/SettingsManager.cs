using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    private Resolution[] resolutions;

    public Toggle toggleFullscreen;
    public Slider sliderVolumeSFX;
    public Slider sliderVolumeBGM;
    public Dropdown dropdownResolutions;

    public GameSettings gameSettings;

    public static SettingsManager _instance;

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


    void Start()
    {
        resolutions = Screen.resolutions;
        gameSettings = new GameSettings();

        toggleFullscreen.onValueChanged.AddListener(OnFullscreenChanged);
        dropdownResolutions.onValueChanged.AddListener(OnResolutionChanged);
        sliderVolumeSFX.onValueChanged.AddListener(OnVolumeChangedSFX);
        sliderVolumeBGM.onValueChanged.AddListener(OnVolumeChangedBGM);


        foreach (Resolution r in resolutions)
        {
            dropdownResolutions.options.Add(new Dropdown.OptionData(r.ToString()));
        }

        LoadSettings();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnFullscreenChanged(bool newValue)
    {
        gameSettings.Fullscreen = Screen.fullScreen = newValue;
    }

    private void OnVolumeChangedBGM(float newValue)
    {
        gameSettings.VolumeBGM = sliderVolumeBGM.value;
        SoundManager._instance.SetBGMVolume(newValue);

        if (newValue == sliderVolumeBGM.minValue)
            SoundManager._instance.MuteBGM();
    }

    private void OnVolumeChangedSFX(float newValue)
    {
        gameSettings.VolumeSFX = sliderVolumeSFX.value;
        SoundManager._instance.SetSFXVolume(newValue);

        if (newValue == sliderVolumeSFX.minValue)
            SoundManager._instance.MuteSFX();
    }

    private void OnResolutionChanged(int newValue)
    {
        Screen.SetResolution(resolutions[newValue].width, resolutions[newValue].height, Screen.fullScreen);
        gameSettings.Resolution = newValue;
    }

    private void LoadSettings()
    {
        try
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

            toggleFullscreen.isOn = gameSettings.Fullscreen;
            sliderVolumeSFX.value = gameSettings.VolumeSFX;
            sliderVolumeBGM.value = gameSettings.VolumeBGM;
            dropdownResolutions.value = gameSettings.Resolution;
        }
        catch(Exception)
        {
            toggleFullscreen.isOn = Screen.fullScreen;
            dropdownResolutions.value = GetResolutionIndex();
            sliderVolumeBGM.value = 1f;
            sliderVolumeSFX.value = 1f;
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(gameSettings, true);

        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", json);
    }

    private int GetResolutionIndex()
    {
        for(int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].ToString() == Screen.currentResolution.ToString())
                return i;
        }

        return 0;
    }
}
