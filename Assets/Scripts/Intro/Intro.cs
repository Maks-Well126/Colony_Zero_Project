using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    private AsyncOperation loadingOperation;
    private bool sceneReady = false;

    void Start()
    {
        videoPlayer.SetDirectAudioMute(0, true);
        StartCoroutine(LoadGameScene());

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    IEnumerator LoadGameScene()
    {
        loadingOperation = SceneManager.LoadSceneAsync(gameSceneName);
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneReady = true;
    }

    void Update()
    {
        if (Input.anyKeyDown && sceneReady)
        {
            ActivateScene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (sceneReady)
        {
            ActivateScene();
        }
    }

    void ActivateScene()
    {
        loadingOperation.allowSceneActivation = true;
    }
}
