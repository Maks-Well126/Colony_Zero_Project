using UnityEngine;
using Player;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private PauseMenuView m_pauseMenu;
    [SerializeField] private DeadScreenView m_deadScreen;

    [SerializeField] private SpawnerEnemy m_spawner;
    [SerializeField] private PlayerController m_player;
    [SerializeField] private Transform m_spawnPoint;
    [SerializeField] private PlayerCameraController m_playerCamera;

    [SerializeField] private GameObject m_RadialMenu;
    [SerializeField] private GameObject m_robotMap;

    private StateMachine m_stateMachine;
    private PlayerInputActions m_actions;



    private void Awake()
    {
        m_stateMachine = new StateMachine();

        m_stateMachine.Initialize(
        new GameplayState(m_spawner),
        new PauseState(m_stateMachine, m_pauseMenu,m_player, m_playerCamera),
        new DeadState(m_stateMachine, m_player, m_spawnPoint, m_spawner, m_deadScreen, m_playerCamera)
        );

        m_player.PlayerDied += OnPlayerDied;

        m_stateMachine.ChangedState<GameplayState>();

        m_actions = new PlayerInputActions();
        m_actions.Player.Enable();

        m_actions.Player.Pause.performed += ctx => OnPausePressed();
    }
    private void OnPausePressed()
    {
        if (m_RadialMenu.gameObject.activeSelf) return;
        if (m_robotMap.activeSelf) return;      
        
        if (m_stateMachine.CurrentState is GameplayState)
        {
            m_stateMachine.ChangedState<PauseState>();
        }
    }

    private void OnPlayerDied()
    {
        m_stateMachine.ChangedState<DeadState>();
    }
}