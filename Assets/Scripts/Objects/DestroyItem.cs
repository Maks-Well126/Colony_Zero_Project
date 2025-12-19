using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestroyItem : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float m_collectionTime = 3f;
    [SerializeField] private float m_interactionDistance = 5f;

    [Header("Completion Settings")]
    [SerializeField] private float m_destroyDelay = 0.3f;

    [Header("Progress Bar")]
    [SerializeField] private GameObject m_progressBarPrefab;

    [Header("Systems")]
    [SerializeField] private ProgressBar m_progressBar;
    [SerializeField] private SoundSystem m_soundSystem;

    private Camera m_playerCamera;
    private AudioSource m_audioSource;
    private bool m_isCollecting = false;
    private float m_currentCollectionTime = 0f;

    private void Start()
    {
        InitializeComponents();
        InitializeSystems();
    }

    private void Update()
    {
        HandleInteraction();
    }

    private void InitializeComponents()
    {
        m_playerCamera = Camera.main;
        m_audioSource = GetComponent<AudioSource>();
    }

    private void InitializeSystems()
    {
        EnsureSystemsExist();
        ConfigureSystems();
    }

    private void EnsureSystemsExist()
    {
        if (m_progressBar == null)
        {
            m_progressBar = gameObject.AddComponent<ProgressBar>();
        }

        if (m_soundSystem == null)
        {
            m_soundSystem = gameObject.AddComponent<SoundSystem>();
        }
    }

    private void ConfigureSystems()
    {
        if (m_progressBarPrefab == null)
        {
            return;
        }

        m_progressBar.SetPrefab(m_progressBarPrefab);
        m_progressBar.Initialize(transform);
        m_progressBar.SetHideDelay(m_destroyDelay);

        m_soundSystem.Initialize(m_audioSource);
    }

    private void HandleInteraction()
    {
        bool isLooking = IsPlayerLookingAtObject();

        if (isLooking)
        {
            ProcessInteraction();
        }
        else if (m_isCollecting)
        {
            StopCollecting();
        }
    }

    private bool IsPlayerLookingAtObject()
    {
        if (m_playerCamera == null) return false;

        Ray ray = new Ray(m_playerCamera.transform.position, m_playerCamera.transform.forward);
        return Physics.Raycast(ray, out RaycastHit hit, m_interactionDistance) &&
               hit.collider.gameObject == gameObject;
    }

    private void ProcessInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && !m_isCollecting)
        {
            StartCollecting();
            return;
        }

        if (Input.GetKeyUp(KeyCode.E) && m_isCollecting)
        {
            StopCollecting();
            return;
        }

        if (Input.GetKey(KeyCode.E) && m_isCollecting)
        {
            UpdateCollecting();
        }
    }

    private void StartCollecting()
    {
        m_isCollecting = true;
        ResetCollectionTimer();

        m_progressBar.StartProgress();
        m_soundSystem.StartCollecting();
    }

    private void UpdateCollecting()
    {
        m_currentCollectionTime += Time.deltaTime;
        float progress = CalculateProgress();

        m_progressBar.UpdateProgress(progress);

        if (IsCollectionComplete())
        {
            CompleteCollecting();
        }
    }

    private void StopCollecting()
    {
        if (!m_isCollecting) return;

        m_isCollecting = false;
        ResetCollectionTimer();

        m_progressBar.CancelProgress();
        m_soundSystem.StopCollecting();
    }

    private void CompleteCollecting()
    {
        m_isCollecting = false;

        m_progressBar.CompleteProgress();
        m_soundSystem.PlayCompleteSound();

        Destroy(gameObject, m_destroyDelay);
    }

    private float CalculateProgress()
    {
        return m_currentCollectionTime / m_collectionTime;
    }

    private bool IsCollectionComplete()
    {
        return m_currentCollectionTime >= m_collectionTime;
    }

    private void ResetCollectionTimer()
    {
        m_currentCollectionTime = 0f;
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        m_soundSystem?.StopAllSounds();
    }
}