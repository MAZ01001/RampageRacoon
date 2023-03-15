using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] Sound[] musicSounds, sfxSounds;
    [SerializeField] AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if(Instance ==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Menu");
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, x=>x.name == name);
        if(sound==null)
        {
            Debug.Log("No Music Sound found (" + name + ")");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfxSounds, x=>x.name == name);
        if(sound==null)
        {
            Debug.Log("No SFX Sound found (" + name + ")");
        }
        else
        {
            sfxSource.PlayOneShot(sound.clip);
            musicSource.Play();
        }
    }

    public void ToggleMasterVolume()
    {
        musicSource.mute=! musicSource.mute;
        sfxSource.mute=! sfxSource.mute;
    }

    public void MasterVolume(float volumeMusic, float volumeSFX, float volumeMaster)
    {
        musicSource.volume = volumeMusic * volumeMaster;
        sfxSource.volume = volumeSFX * volumeMaster;
    }
    
    public void MusicVolume(float volume, float volumeMaster)
    {
        musicSource.volume = volume * volumeMaster;
    }

    public void SFXVolume(float volume, float volumeMaster)
    {
        sfxSource.volume = volume * volumeMaster;
    }
}

    