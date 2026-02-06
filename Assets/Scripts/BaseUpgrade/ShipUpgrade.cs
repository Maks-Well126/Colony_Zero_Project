using UnityEngine;

public class ShipUpgrade : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private Transform buildingSpawnPoint;

    [Header("UI")]
    [SerializeField] private GameObject upgradeHintUI;

    private bool playerNearby;
    private bool upgraded;

    private void Start()
    {
        if (upgradeHintUI != null)
            upgradeHintUI.SetActive(false);
    }

    private void Update()
    {
        if (upgraded) return;

        // 🔹 Иконка появляется ТОЛЬКО при подходе
        if (playerNearby && Artifact.m_isDelivered)
        {
            upgradeHintUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                UpgradeShip();
            }
        }
        else
        {
            upgradeHintUI.SetActive(false);
        }
    }

    private void UpgradeShip()
    {
        Instantiate(buildingPrefab, buildingSpawnPoint.position, buildingSpawnPoint.rotation);
        upgraded = true;
        upgradeHintUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
