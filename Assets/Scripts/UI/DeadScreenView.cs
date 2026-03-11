using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadScreenView : MonoBehaviour
{
    [SerializeField] private Button m_respawnButton;
    [SerializeField] private Button m_exitButton;

    public event Action RespawnClicked;

    private void OnEnable()
    {
        m_respawnButton.onClick.AddListener(OnRespawnClicked);
        m_exitButton.onClick.AddListener(ExitMainMenu);
    }

    private void OnDisable()
    {
        m_respawnButton.onClick.RemoveListener(OnRespawnClicked);
        m_exitButton.onClick.RemoveListener(ExitMainMenu);
    }

    private void OnRespawnClicked()
    {
        RespawnClicked?.Invoke();
    }

    private void ExitMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}