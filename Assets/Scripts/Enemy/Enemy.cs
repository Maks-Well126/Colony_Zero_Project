using System;
using UnityEngine;

public sealed class Enemy : MonoBehaviour
{
    public event Action<Enemy> Died;

    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private EnemyAttack m_attack;
    [SerializeField] private HealthComponent m_health;
    [SerializeField] private EnemyAnimator m_animator;
    [SerializeField] private EnemyAmbientAudio m_ambientAudio;

    private EnemyData m_data;
    private Transform m_player;
    private EnemyStateMachine m_stateMachine;

    private void Awake()
    {
        m_stateMachine = new EnemyStateMachine();

        m_attack.OnAttackStarted += () => m_animator.PlayAttack();
    }

    private void OnEnable()
    {
        m_health.Died += OnDied;
        m_health.Damaged += OnDamaged;
        m_stateMachine.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        m_health.Died -= OnDied;
        m_health.Damaged -= OnDamaged;
        m_stateMachine.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (m_stateMachine.currentState == EnemyState.Dead || !m_data)
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
        if (m_ambientAudio)
            m_ambientAudio.Initialize(player);

        m_stateMachine.ChangeState(EnemyState.Idle);
    }

    private void UpdateState()
    {
        float distance = Vector3.Distance(transform.position, m_player.position);

        switch (m_stateMachine.currentState)
        {
            case EnemyState.Idle:
                if (distance <= m_data.detectRange)
                    m_stateMachine.ChangeState(EnemyState.Move);
                break;

            case EnemyState.Move:
                if (distance <= m_data.attackRange)
                    m_stateMachine.ChangeState(EnemyState.Attack);
                break;

            case EnemyState.Attack:
                if (distance > m_data.attackRange)
                {
                    m_stateMachine.ChangeState(EnemyState.Move);
                    break;
                }

                m_attack.TryAttack();
                break;
        }
    }

    private void OnStateChanged(EnemyState prev, EnemyState next)
    {
        switch (next)
        {
            case EnemyState.Idle:
                m_animator.SetMoveSpeed(0f);
                m_movement.StopMoving();
                break;

            case EnemyState.Move:
                m_animator.SetMoveSpeed(1f);
                m_movement.StartMoving();
                break;

            case EnemyState.Attack:
                m_animator.SetMoveSpeed(0f);
                m_animator.PlayAttack();
                m_movement.StopMoving();
                break;

            case EnemyState.Dead:
                m_animator.PlayDead();
                m_movement.StopMoving();
                break;
        }
    }

    private void OnDamaged()
    {
        m_animator.PlayHit();
    }

    private void OnDied()
    {
        EnemyAmbientAudio ambient = GetComponent<EnemyAmbientAudio>();
        if (ambient)
            ambient.enabled = false;

        m_stateMachine.ChangeState(EnemyState.Dead);
        Died?.Invoke(this);
    }
}
