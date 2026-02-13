using UnityEngine;

public sealed class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_animator;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    //private static readonly int Hit = Animator.StringToHash("Hit");

    public void SetMoveSpeed(float value)
    {
        m_animator.SetFloat(Speed, value);
    }

    public void PlayAttack()
    {
        m_animator.SetTrigger(Attack);
    }

    public void PlayHit()
    {
        m_animator.SetTrigger("Hit");
    }


    public void PlayDead()
    {
        m_animator.SetBool(IsDead, true);
    }
}
