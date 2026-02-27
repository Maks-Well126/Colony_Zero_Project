using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip[] buttonClickSounds;
    [SerializeField] private AudioClip sliderSound;
    [SerializeField] private AudioClip popupOpenSound;
    [SerializeField] private AudioClip popupCloseSound;
    [SerializeField] private AudioClip errorSound;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
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
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource) sfxSource.volume = sfxVolume;
    }

    public void PlayButtonHover() => PlaySFX(buttonHoverSound);

    public void PlayButtonClick(int index = 0)
    {
        if (buttonClickSounds != null && buttonClickSounds.Length > 0)
        {
            if (index >= 0 && index < buttonClickSounds.Length)
            {
                PlaySFX(buttonClickSounds[index]);
            }
            else
            {
                PlaySFX(buttonClickSounds[0]);
            }
        }
    }

    public void PlaySliderSound() => PlaySFX(sliderSound);
    public void PlayPopupOpen() => PlaySFX(popupOpenSound);
    public void PlayPopupClose() => PlaySFX(popupCloseSound);
    public void PlayError() => PlaySFX(errorSound);

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic() => musicSource?.Stop();

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource) sfxSource.volume = volume;
    }
}