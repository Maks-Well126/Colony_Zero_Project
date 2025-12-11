using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator m_animator;

        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void SetMoveSpeed(float value)
        {
            m_animator.SetFloat(MoveSpeed, value);
        }

        public void SetRunning(bool value)
        {
            m_animator.SetBool(IsRunning, value);
        }
    }
}
