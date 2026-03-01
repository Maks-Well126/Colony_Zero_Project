using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_exitButton;

    private void Awake()
    {
        m_playButton.onClick.AddListener(LoadGame);
        m_exitButton.onClick.AddListener(ExitGame);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}