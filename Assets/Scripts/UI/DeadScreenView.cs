using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadScreenView : MonoBehaviour
{
    [SerializeField] private Button m_respawnButton;
    [SerializeField] private Button m_exitButton;
    public event Action RespawnClicked;

    private void Awake()
    {
        m_respawnButton.onClick.AddListener(() => RespawnClicked?.Invoke());
        m_exitButton.onClick.AddListener(ExitMainMenu);
    }
   
    private void ExitMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }


}