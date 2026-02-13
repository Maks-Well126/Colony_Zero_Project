using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_agent;

    private Transform m_target;
    private bool m_isMoving;
    private bool m_isInitialized;

    private void OnValidate()
    {
        if (!m_agent)
            m_agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(float speed, Transform target)
    {
        m_target = target;
        m_agent.speed = speed;
        m_isInitialized = true;
    }

    private void Update()
    {
        if (!m_isInitialized || !m_isMoving || !m_target)
            return;

        m_agent.SetDestination(m_target.position);
    }

    private void LateUpdate()
    {
        if (!m_isInitialized || m_isMoving || !m_target)
            return;

        Vector3 dir = m_target.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 8f
        );
    }

    public void StartMoving()
    {
        if (!m_isInitialized)
            return;

        m_isMoving = true;
        m_agent.isStopped = false;
    }

    public void StopMoving()
    {
        if (!m_isInitialized)
            return;

        m_isMoving = false;
        m_agent.isStopped = true;
        m_agent.velocity = Vector3.zero;
    }
}