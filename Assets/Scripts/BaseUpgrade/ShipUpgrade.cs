using UnityEngine;
using UnityEngine.InputSystem;

public class ShipUpgrade : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private GameObject m_buildingPrefab;
    [SerializeField] private Transform m_buildingSpawnPoint;

    [SerializeField] private GameObject m_buildingPrefab2;
    [SerializeField] private Transform m_buildingSpawnPoint2;

    [Header("UI")]
    [SerializeField] private GameObject m_radialUI;

    private bool playerNearby;
    private bool isLevel1Built = false;
    private bool isLevel2Built = false;
    private bool m_debugMode = true;

    private void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
            MouseDirectionChecker.GetMouseDirection(0.3f, 50);

        if (direction == MouseDirectionChecker.MouseDirection.Up)
        {
            HandleUpgrade();
        }

        HandleUI();
    }

    private void HandleUpgrade()
    {
        if (Artifact.isArtefact1Delivered && !isLevel1Built)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AudioManager.Instance.PlayButtonClick(2);
                UpgradeShip(m_buildingPrefab, m_buildingSpawnPoint);
                isLevel1Built = true;
            }
        }
        else if (Artifact.isArtefact2Delivered && !isLevel2Built)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AudioManager.Instance.PlayButtonClick(2);
                UpgradeShip(m_buildingPrefab2, m_buildingSpawnPoint2);
                isLevel2Built = true;
            }
        }
        if (!(Artifact.isArtefact1Delivered && !isLevel1Built))
        {
            if (Input.GetMouseButtonDown(0))
            {
                AudioManager.Instance.PlayButtonClick(3);                
            }
        }
    }

    private void HandleUI()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerNearby)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_radialUI.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_radialUI.gameObject.SetActive(false);
        }
        else if (!playerNearby && m_radialUI.activeSelf)
        {
            m_radialUI.gameObject.SetActive(false);
        }
    }

    private void UpgradeShip(GameObject prefab, Transform position)
    {
        if (prefab != null && position != null)
        {
            Instantiate(prefab, position.position, position.rotation);
        }
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