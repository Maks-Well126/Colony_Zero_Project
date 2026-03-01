using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_exitButton;

    public event Action ResumeClicked;
    public event Action ExitClicked;

    private void Awake()
    {
        m_resumeButton.onClick.AddListener(() => ResumeClicked?.Invoke());
        m_exitButton.onClick.AddListener(() => ExitClicked?.Invoke());
    }
}