using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] Toggle masterVolume;
    private string toggleAn;

    private void Start()
    {
        if(PlayerPrefs.GetString("MasterToggle")=="true")
        {
            masterVolume.isOn=true;
        }
        else if(PlayerPrefs.GetString("MasterToggle")=="false")
        {
            masterVolume.isOn=false;
        }
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void Update()
    {
        if(masterVolume.isOn==true)
        {
            toggleAn="true";
        }
        else
        {
            toggleAn="false";
        }
        PlayerPrefs.SetString("MasterToggle", toggleAn);
        
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void ToggleMasterVolume()
    {
        SoundManager.Instance.ToggleMasterVolume(masterVolume.isOn);
    }

    public void MasterVolume()
    {
        SoundManager.Instance.MasterVolume(musicSlider.value, sfxSlider.value, masterSlider.value);
    }
    
    public void MusicVolume()
    {
        SoundManager.Instance.MusicVolume(musicSlider.value, masterSlider.value);
    }

    public void SFXVolume()
    {
        SoundManager.Instance.SFXVolume(sfxSlider.value, masterSlider.value);
    }
}
