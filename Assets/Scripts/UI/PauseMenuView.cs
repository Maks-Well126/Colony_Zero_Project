using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_exitButton;

    public event Action ResumeClicked;
    public event Action ExitClicked;

    private void OnEnable()
    {
        m_resumeButton.onClick.AddListener(OnResumeClicked);
        m_exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDisable()
    {
        m_resumeButton.onClick.RemoveListener(OnResumeClicked);
        m_exitButton.onClick.RemoveListener(OnExitClicked);
    }

    private void OnResumeClicked()
    {
        ResumeClicked?.Invoke();
    }

    private void OnExitClicked()
    {
        ExitClicked?.Invoke();
    }
}