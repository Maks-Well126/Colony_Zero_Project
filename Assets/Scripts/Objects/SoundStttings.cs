using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundSettings", menuName = "Destroy/Sound Settings")]
public class SoundSettings : ScriptableObject
{
    [Header("Звуковые файлы")]
    public AudioClip m_collectingSound;
    public AudioClip m_completeSound;

    [Header("Настройки звука")]
    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0f, 1f)] public float collectingVolumeMultiplier = 1f;
    [Range(0f, 1f)] public float spatialBlend = 1f;
    public float maxDistance = 10f;

    [Header("Дополнительные настройки")]
    public bool loopCollectingSound = true;
    public float pitchVariation = 0.1f;
}
