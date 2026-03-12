using UnityEngine;
using UnityEngine.SceneManagement;
using static DialogeSystem;

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

    private void Awake()
    {
        isLevel1Built = Save.LoadLevel1State();
        isLevel2Built = Save.LoadLevel2State();
    }
    private void Start()
    {
        if (isLevel1Built)
        {
            Instantiate(m_buildingPrefab, m_buildingSpawnPoint.position, m_buildingSpawnPoint.rotation);
            Instantiate(m_buildingPrefab2, m_buildingSpawnPoint2.position, m_buildingSpawnPoint2.rotation);
        }
    }

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
                UpgradeShip(m_buildingPrefab, m_buildingSpawnPoint, m_buildingPrefab2, m_buildingSpawnPoint2);
                isLevel1Built = true;
                DialogeSystem.StartDialoge(DialogType.Build);

                Save.SaveLevel1State(true);
            }
        }
        else if (Artifact.isArtefact2Delivered && !isLevel2Built)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DialogeSystem.StartDialoge(DialogType.Build);
                SceneManager.LoadScene("End");
                isLevel2Built = true;                
                Save.SaveLevel2State(true);
            }
        }        
    }

    private void HandleUI()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerNearby)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            m_radialUI.gameObject.SetActive(true);
        }        
        else if (!playerNearby && m_radialUI.activeSelf)
        {
            m_radialUI.gameObject.SetActive(false);
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void UpgradeShip(GameObject prefab, Transform position, GameObject prefab2, Transform position2)
    {
        Instantiate(prefab, position.position, position.rotation);
        Instantiate(prefab2, position2.position, position2.rotation);
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