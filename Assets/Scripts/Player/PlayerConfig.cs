using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player Config")]
public sealed class PlayerConfig : ScriptableObject
{
    [Header("Health")]
    [SerializeField][Min(1)] private float m_maxHealth = 100f;

    [Header("Movement")]
    [SerializeField][Min(0.1f)] private float m_moveSpeed = 5f;
    [SerializeField][Min(0.1f)] private float m_runSpeed = 9f;
    [SerializeField][Min(0.1f)] private float m_jumpHeight = 1.5f;
    [SerializeField] private float m_gravity = -9.81f;

    [Header("Aim")]
    [SerializeField][Min(1f)] private float m_aimDistance = 10f;
    [SerializeField][Min(1f)] private float m_rigSmoothSpeed = 8f;
    [Header("Audio")]
    [SerializeField] private AudioClip[] m_hitSounds;
    [SerializeField] private AudioClip m_deadAudio;

    public AudioClip[] HitSounds => m_hitSounds;
    public AudioClip DeadAudio => m_deadAudio;

    public float MaxHealth => m_maxHealth;

    public float MoveSpeed => m_moveSpeed;
    public float RunSpeed => m_runSpeed;
    public float JumpHeight => m_jumpHeight;
    public float Gravity => m_gravity;

    public float AimDistance => m_aimDistance;
    public float RigSmoothSpeed => m_rigSmoothSpeed;
}