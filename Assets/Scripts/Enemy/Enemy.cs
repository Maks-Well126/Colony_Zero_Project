using System;
using UnityEngine;

public sealed class Enemy : MonoBehaviour
{
    public event Action<Enemy> Died;

    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private EnemyAttack m_attack;
    [SerializeField] private HealthComponent m_health;

    private EnemyData m_data;
    private Transform m_player;
    private EnemyStateMachine m_stateMachine;

    private void Awake()
    {
        m_stateMachine = new EnemyStateMachine();
    }

    private void OnEnable()
    {
        m_health.Died += OnDied;
        m_stateMachine.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        m_health.Died -= OnDied;
        m_stateMachine.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (m_stateMachine.currentState is EnemyState.Dead || !m_data)
            return;

        UpdateState();
    }

    public void Initialize(EnemyData data, Transform player)
    {
        m_data = data;
        m_player = player;

        m_health.Initialize(data.health);
        m_movement.Initialize(data.moveSpeed, player);
        m_attack.Initialize(data.damage, data.attackCooldown, player);

        m_stateMachine.ChangeState(EnemyState.Move);
    }

    private void UpdateState()
    {
        bool inRange = IsInAttackRange();

        switch (m_stateMachine.currentState)
        {
            case EnemyState.Move:
                if (inRange)
                    m_stateMachine.ChangeState(EnemyState.Attack);
                break;

            case EnemyState.Attack:
                m_attack.TryAttack();

                if (!inRange)
                    m_stateMachine.ChangeState(EnemyState.Move);
                break;
        }
    }

    private bool IsInAttackRange()
    {
        if (!m_player)
            return false;

        return Vector3.Distance(transform.position, m_player.position)
               <= m_data.attackRange;
    }

    private void OnStateChanged(EnemyState prev, EnemyState next)
    {
        if (prev == EnemyState.Move)
            m_movement.StopMoving();

        if (next == EnemyState.Move)
            m_movement.StartMoving();
    }

    private void OnDied()
    {
        m_stateMachine.ChangeState(EnemyState.Dead);
        Died?.Invoke(this);
    }
}