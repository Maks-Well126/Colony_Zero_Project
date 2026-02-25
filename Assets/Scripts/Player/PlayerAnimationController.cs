using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator m_animator;

        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int DeathHash = Animator.StringToHash("Death");

        private static readonly int IsAiming = Animator.StringToHash("IsAiming");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");
        private static readonly int ReloadHash = Animator.StringToHash("Reload");

        public void TriggerReload()
        {
            m_animator.SetTrigger(ReloadHash);
        }


        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void SetMoveDirection(Vector2 dir)
        {
            m_animator.SetFloat(MoveX, dir.x, 0.1f, Time.deltaTime);
            m_animator.SetFloat(MoveY, dir.y, 0.1f, Time.deltaTime);
        }
        public void SetMove(float value)
        {
            m_animator.SetFloat(MoveSpeed, value, 0.1f, Time.deltaTime);
        }

        public void SetRunning(bool value)
        {
            m_animator.SetBool(IsRunning, value);
        }

        public void SetAiming(bool value)
        {
            m_animator.SetBool(IsAiming, value);
        }

        public void Shoot()
        {
            m_animator.SetTrigger(ShootHash);
        }
        public void Jump()
        {
            m_animator.SetTrigger(JumpHash);
        }

        public void SetGrounded(bool value)
        {
            m_animator.SetBool(IsGroundedHash, value);
        }

        public void TriggerDeath()
        {
            m_animator.SetTrigger(DeathHash);
        }
        public void TriggerHit()
        {
            if (IsInHitState())
                return;

            m_animator.SetTrigger(HitHash);
        }

        private bool IsInHitState()
        {
            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Hit");
        }

    }
}
