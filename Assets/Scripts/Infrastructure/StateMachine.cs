using System;
using System.Collections.Generic;
using Player;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMachine
{
    private IState m_state;
    private Dictionary<Type, IState> m_states = new();
    public IState CurrentState => m_state;


    public void Initialize(params IState[] states)
    {
        if (m_states.Count > 0) return;

        foreach (var state in states)
        {
            m_states.Add(state.GetType(), state);
        }
    }

    public void ChangedState<T>()
        where T : IState
    {
        m_state?.Exit();
        {
            m_state = m_states[typeof(T)];
        }
        m_state.Enter();
    }
}

public class GameplayState : IState
{
    private readonly SpawnerEnemy m_spawner;

    public GameplayState(SpawnerEnemy spawner)
    {
        m_spawner = spawner;
    }

    public void Enter()
    {
        Time.timeScale = 1f;
        m_spawner.Spawn();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Exit()
    {

    }
}

public class PauseState : IState
{
    private readonly StateMachine m_stateMachine;
    private readonly PauseMenuView m_view;
    private readonly PlayerCameraController m_cameraController;

    public PauseState(StateMachine stateMachine, PauseMenuView view, PlayerCameraController cameraController)
    {
        m_stateMachine = stateMachine;
        m_view = view;
        m_cameraController = cameraController;

        m_view.gameObject.SetActive(false);
    }

    public void Enter()
    {
        Time.timeScale = 0f;

        m_view.gameObject.SetActive(true);
        m_view.ResumeClicked += OnResume;
        m_view.ExitClicked += OnExit;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_cameraController.enabled = false;
    }

    public void Exit()
    {
        Time.timeScale = 1f;

        m_view.ResumeClicked -= OnResume;
        m_view.ExitClicked -= OnExit;
        m_view.gameObject.SetActive(false);

        m_cameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnResume() =>
        m_stateMachine.ChangedState<GameplayState>();

    private void OnExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}

public class DeadState : IState
{
    private readonly StateMachine m_stateMachine;
    private readonly PlayerController m_player;
    private readonly Transform m_spawnPoint;
    private readonly SpawnerEnemy m_spawner;
    private readonly DeadScreenView m_deadScreenView;
    private readonly PlayerCameraController m_cameraController;

    public DeadState(
        StateMachine stateMachine,
        PlayerController player,
        Transform spawnPoint,
        SpawnerEnemy spawner,
        DeadScreenView deadScreenView,
        PlayerCameraController cameraController)
    {
        m_stateMachine = stateMachine;
        m_player = player;
        m_spawnPoint = spawnPoint;
        m_spawner = spawner;
        m_deadScreenView = deadScreenView;
        m_cameraController = cameraController;

        m_deadScreenView.gameObject.SetActive(false);
    }

    public void Enter()
    {
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_deadScreenView.gameObject.SetActive(true);
        m_deadScreenView.RespawnClicked += OnRespawnClicked;
        m_cameraController.enabled = false;
    }

    public void Exit()
    {
        m_deadScreenView.gameObject.SetActive(false);
        Time.timeScale = 1f;
        m_deadScreenView.RespawnClicked -= OnRespawnClicked;
        m_spawner.ClearAll();
        m_cameraController.enabled = true;
    }
    private void OnRespawnClicked()
    {
        m_player.transform.position = m_spawnPoint.position;
        m_player.Respawn(m_spawnPoint.position);

        m_stateMachine.ChangedState<GameplayState>();
    }

    public void Respawn()
    {
        m_player.transform.position = m_spawnPoint.position;
        m_player.Respawn(m_spawnPoint.position);

        m_stateMachine.ChangedState<GameplayState>();

    }
}