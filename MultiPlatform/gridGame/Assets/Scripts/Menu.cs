﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public int[] screenWidths;
    private int activeScreenResIndex;

    
    void Start () {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1)? true: false;

        volumeSliders[0].value = AudioManager.instance.masterVolumPercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        fullscreenToggle.isOn = isFullscreen;
    }

    void Update () {
		
	}

    public void Play(){
        SceneManager.LoadScene("Test");
    }

    public void Quit(){
        Application.Quit();
    }

    public void OptionMenu(){
        mainMenuHolder.SetActive(false);
        optionMenuHolder.SetActive(true);
    }

    public void MainMenu(){
        mainMenuHolder.SetActive(true);
        optionMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i){
        if (resolutionToggles[i].isOn){
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i]/aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float value){
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value){
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfcVolume(float value){
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void SetFullscreen (bool isFullscreen){
        for (int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].interactable = !isFullscreen;
        }
        if (isFullscreen){
            Resolution[] allResolutuins = Screen.resolutions;
            Resolution maxResolution = allResolutuins[allResolutuins.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else{
            SetScreenResolution(activeScreenResIndex);
        }
        PlayerPrefs.SetInt("fullscreen", ((isFullscreen)? 1:0));
        PlayerPrefs.Save();
    }
}
