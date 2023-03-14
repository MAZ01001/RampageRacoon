using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] Toggle masterVolume;

    public void ToggleMasterVolume()
    {
        SoundManager.Instance.ToggleMasterVolume();
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
