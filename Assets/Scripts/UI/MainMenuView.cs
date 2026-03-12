using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_exitButton;

    private void OnEnable()
    {
        m_playButton.onClick.AddListener(LoadGame);
        m_exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        m_playButton.onClick.RemoveListener(LoadGame);
        m_exitButton.onClick.RemoveListener(ExitGame);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Intro");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}