using UnityEngine;
using UnityEngine.Audio;

public class RobotRepair : MonoBehaviour
{
    [Header("Repair Settings")]
    [SerializeField] private float m_repairTime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip m_processSound;

    [SerializeField] private RobotController m_robot;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private float m_timer;
    private bool m_isRepairing;

    private AudioSource audioSource;

    private void Awake()
    {
        if (m_robot == null)
            m_robot = GetComponent<RobotController>();
   

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        
        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup; 
    }

    public bool CanRepair => m_robot != null && m_robot.IsBroken && !m_isRepairing;

    public void StartRepair()
    {
        if (m_isRepairing || !m_robot.IsBroken)
            return;

        m_isRepairing = true;
        m_timer = 0;

        if (m_processSound != null)
        {
            audioSource.clip = m_processSound;
            audioSource.Play();
        }
    }

    public void UpdateRepair(float deltaTime)
    {
        if (!m_isRepairing || !m_robot.IsBroken)
            return;

        m_timer += deltaTime;

        if (m_timer >= m_repairTime)
        {
            CompleteRepair();
        }
    }

    public void CancelRepair()
    {
        if (!m_isRepairing)
            return;

        m_isRepairing = false;
        m_timer = 0;
        audioSource.Stop();
    }

    private void CompleteRepair()
    {
        m_isRepairing = false;

        audioSource.Stop();

        m_robot.RepairRobotInField();
    }
}