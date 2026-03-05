using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource sfxSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip[] buttonClickSounds;
    [SerializeField] private AudioClip sliderSound;
    [SerializeField] private AudioClip popupOpenSound;
    [SerializeField] private AudioClip popupCloseSound;
    [SerializeField] private AudioClip errorSound;

    [Header("Music")]
    [SerializeField] private AudioClip[] MusicSounds;

    [Header("Scene Music")]
    [SerializeField] private string[] sceneNames;
    [SerializeField] private int[] sceneMusicIndexes;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.025f;
    [SerializeField] private float fadeTime = 2f;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private int currentMusicIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentSource = musicSourceA;
            nextSource = musicSourceB;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (musicSourceA) musicSourceA.volume = musicVolume;
        if (musicSourceB) musicSourceB.volume = 0f;
        if (sfxSource) sfxSource.volume = sfxVolume;

        CheckSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneMusic(scene.name);
    }

    private void CheckSceneMusic(string sceneName)
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneName == sceneNames[i])
            {
                PlayMusic(sceneMusicIndexes[i]);
                return;
            }
        }
    }

    public void PlayButtonHover() => PlaySFX(buttonHoverSound);

    public void PlayButtonClick(int index = 0)
    {
        if (buttonClickSounds != null && buttonClickSounds.Length > 0)
        {
            if (index >= 0 && index < buttonClickSounds.Length)
                PlaySFX(buttonClickSounds[index]);
            else
                PlaySFX(buttonClickSounds[0]);
        }
    }

    public void PlaySliderSound() => PlaySFX(sliderSound);
    public void PlayPopupOpen() => PlaySFX(popupOpenSound);
    public void PlayPopupClose() => PlaySFX(popupCloseSound);
    public void PlayError() => PlaySFX(errorSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(int clipIndex)
    {
        if (MusicSounds == null || MusicSounds.Length == 0) return;
        if (clipIndex < 0 || clipIndex >= MusicSounds.Length) return;
        if (clipIndex == currentMusicIndex) return;

        currentMusicIndex = clipIndex;

        StopAllCoroutines();
        StartCoroutine(CrossFade(MusicSounds[clipIndex]));
    }

    private IEnumerator CrossFade(AudioClip newClip)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.loop = true;
        nextSource.Play();

        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            float t = time / fadeTime;

            currentSource.volume = Mathf.Lerp(musicVolume, 0, t);
            nextSource.volume = Mathf.Lerp(0, musicVolume, t);

            yield return null;
        }

        currentSource.Stop();

        var temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }

    public void StopMusic()
    {
        currentSource?.Stop();
        nextSource?.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (currentSource) currentSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource) sfxSource.volume = volume;
    }
}