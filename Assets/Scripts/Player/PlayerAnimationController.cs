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
        private static readonly int IsAiming = Animator.StringToHash("IsAiming");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");

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
    }
}
