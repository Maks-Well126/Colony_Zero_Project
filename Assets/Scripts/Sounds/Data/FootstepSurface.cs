using UnityEngine;

[CreateAssetMenu(fileName = "FootstepSurface", menuName = "Xlab/Audio/Footstep Surface")]
public sealed class FootstepSurface : ScriptableObject
{
    [Header("Terrain")]
    [SerializeField] private string m_layerName;

    [Header("Audio")]
    [SerializeField] private AudioClip[] m_clips;

    [SerializeField][Range(0.8f, 1.2f)]
    private float m_minPitch = 0.95f;

    [SerializeField][Range(0.8f, 1.2f)]
    private float m_maxPitch = 1.05f;

    public string layerName => m_layerName;
    public AudioClip[] clips => m_clips;
    public float minPitch => m_minPitch;
    public float maxPitch => m_maxPitch;
}