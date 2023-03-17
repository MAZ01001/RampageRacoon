using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] Sound[] musicSounds, sfxSounds;
    [SerializeField] AudioSource musicSource1, musicSource2, sfxSource;

    private DrugEffectManager drugEffectManager;

    private float m1VolFloat = 1;
    private float m2VolFloat = 0;

    private float musicTotalVolumeBuffer = 0;

    private void Awake()
    {
        if(Instance == null)
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
        
        PlayMusic("Light");
        PlayMusic("Dark");


    }

    private void FixedUpdate()
    {
        if ("Level" == SceneManager.GetActiveScene().name)
        {
            drugEffectManager = GameManager.Instance.Player.gameObject.GetComponent<DrugEffectManager>();
            CrossFadeMusicTracks(drugEffectManager.GetEnvironmentEffect());
        }
        else { m1VolFloat = 1; m2VolFloat = 0; }
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, x=>x.name == name);
        Debug.Log(musicSounds.Length);
        Debug.Log(sound.name);
        if(sound==null)
        {
            Debug.Log("No Music Sound found (" + name + ")");
        }
        else
        {
            if (!musicSource1.isPlaying)
            {
                musicSource1.clip = sound.clip;
                musicSource1.Play();
            }
            else if (musicSource1.isPlaying)
            {
                musicSource2.clip = sound.clip;
                musicSource2.Play();
            }
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
            sfxSource.Play();
        }
    }

    public void ToggleMasterVolume(bool isOn)
    {
        if(isOn)
        {
            musicSource1.mute=false;
            musicSource2.mute = false;
            sfxSource.mute=false;    
        }
        else if(!isOn)
        {
            musicSource1.mute=true;
            musicSource2.mute = true;
            sfxSource.mute=true;
        }
    }

    public void MasterVolume(float volumeMusic, float volumeSFX, float volumeMaster)
    {
        musicSource1.volume = volumeMusic * volumeMaster * m1VolFloat;
        musicSource2.volume = volumeMusic * volumeMaster * m2VolFloat;
        sfxSource.volume = volumeSFX * volumeMaster;
        musicTotalVolumeBuffer = volumeMusic * volumeMaster;
    }
    
    public void MusicVolume(float volume, float volumeMaster)
    {
        musicSource1.volume = volume * volumeMaster * m1VolFloat;
        musicSource2.volume = volume * volumeMaster * m2VolFloat;
        musicTotalVolumeBuffer = volume * volumeMaster;
    }

    public void SFXVolume(float volume, float volumeMaster)
    {
        sfxSource.volume = volume * volumeMaster;
    }
    private void CrossFadeMusicTracks(float blendFloat)
    {
        m1VolFloat = blendFloat;
        m2VolFloat = 1 - blendFloat;
        musicSource1.volume = musicTotalVolumeBuffer * m1VolFloat;
        musicSource2.volume = musicTotalVolumeBuffer * m2VolFloat;
    }
}

    