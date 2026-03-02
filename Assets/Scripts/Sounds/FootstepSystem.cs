using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class FootstepSystem : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private FootstepDatabase m_database;

    [Header("Raycast")]
    [SerializeField] private Transform m_rayOrigin;
    [SerializeField][Min(0.1f)] private float m_rayDistance = 2f;

    [Header("Step Settings")]
    [SerializeField] private float m_walkStepDistance = 2f;
    [SerializeField] private float m_runStepDistance = 1.4f;

    private AudioSource m_audioSource;
    private Terrain m_terrain;

    private Vector3 m_lastPosition;
    private float m_distanceCounter;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_terrain = Terrain.activeTerrain;
        m_lastPosition = transform.position;
    }

    private void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, m_lastPosition);
        m_lastPosition = transform.position;

        m_distanceCounter += distanceMoved;
    }

    public void TryPlayStep(bool isRunning, bool isGrounded, float moveAmount)
    {
        if (!isGrounded)
            return;

        if (moveAmount < 0.1f)
            return;

        float stepDistance = isRunning ? m_runStepDistance : m_walkStepDistance;

        if (m_distanceCounter >= stepDistance)
        {
            m_distanceCounter = 0f;
            PlayFootstep();
        }
    }
    
    public void PlayFootstep()
    {
        if (m_database == null || m_terrain == null)
            return;

        if (!Physics.Raycast(m_rayOrigin.position, Vector3.down, out RaycastHit hit, m_rayDistance))
            return;

        Vector3 terrainPos = hit.point - m_terrain.transform.position;
        TerrainData data = m_terrain.terrainData;

        int mapX = Mathf.FloorToInt((terrainPos.x / data.size.x) * data.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / data.size.z) * data.alphamapHeight);

        float[,,] splatmapData = data.GetAlphamaps(mapX, mapZ, 1, 1);

        int dominantLayer = 0;
        float maxMix = 0f;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                dominantLayer = i;
                maxMix = splatmapData[0, 0, i];
            }
        }

        string layerName = data.terrainLayers[dominantLayer].name;

        FootstepSurface surface = m_database.GetSurface(layerName);

        if (surface == null || surface.clips == null || surface.clips.Length == 0)
            return;

        AudioClip clip = surface.clips[Random.Range(0, surface.clips.Length)];

        m_audioSource.pitch = Random.Range(surface.minPitch, surface.maxPitch);
        m_audioSource.PlayOneShot(clip);
    }
}