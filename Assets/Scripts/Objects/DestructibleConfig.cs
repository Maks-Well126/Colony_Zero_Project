using UnityEngine;

[CreateAssetMenu(
    fileName = "DestructibleConfig",menuName = "Game/Destructible Config")]
public sealed class DestructibleConfig : ScriptableObject
{
    [Header("Destroy")]
    [SerializeField][Min(0.1f)] private float m_destroyTime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip m_loopSound;
    [SerializeField] private AudioClip m_completeSound;

    public float DestroyTime => m_destroyTime;
    public AudioClip LoopSound => m_loopSound;
    public AudioClip CompleteSound => m_completeSound;
}
