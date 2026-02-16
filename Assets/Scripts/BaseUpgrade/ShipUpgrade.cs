using UnityEngine;

public class ShipUpgrade : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private GameObject m_buildingPrefab;
    [SerializeField] private Transform m_buildingSpawnPoint;

    [SerializeField] private GameObject m_buildingPrefab2;
    [SerializeField] private Transform m_buildingSpawnPoint2;

    [Header("UI")]
    [SerializeField] private GameObject m_upgradeHintUI;

    private bool playerNearby;

    // Flags to prevent building multiple times
    private bool isLevel1Built = false;
    private bool isLevel2Built = false;

    private void Start()
    {
        if (m_upgradeHintUI != null)
            m_upgradeHintUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby)
        {
            if (m_upgradeHintUI.activeSelf)
                m_upgradeHintUI.SetActive(false);
            return;
        }

        // Level 1 upgrade
        if (Artifact.isArtefact1Delivered && !isLevel1Built)
        {
            m_upgradeHintUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                UpgradeShip(m_buildingPrefab, m_buildingSpawnPoint);
                isLevel1Built = true; // mark as built
                m_upgradeHintUI.SetActive(false);
            }
        }
        // Level 2 upgrade
        else if (Artifact.isArtefact2Delivered && !isLevel2Built)
        {
            m_upgradeHintUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                UpgradeShip(m_buildingPrefab2, m_buildingSpawnPoint2);
                isLevel2Built = true; // mark as built
                m_upgradeHintUI.SetActive(false);
            }
        }
        else
        {
            if (m_upgradeHintUI.activeSelf)
                m_upgradeHintUI.SetActive(false);
        }
    }

    private void UpgradeShip(GameObject prefab, Transform position)
    {
        Instantiate(prefab, position.position, position.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
