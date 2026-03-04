using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float m_distance = 3f;
    [SerializeField] private LayerMask m_layer;
    [SerializeField] private TMP_Text m_interactionText;
    [SerializeField] private KeyCode m_key = KeyCode.E;
    

    private Camera m_camera;
    private Destructible m_current;

    private void Start()
    {
        m_camera = Camera.main;
        m_interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckObject();
        HandleInput();
    }

    private void CheckObject()
    {
        Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, m_distance))
        {
            m_current = hit.collider.GetComponent<Destructible>();
        }
        else
        {
            m_current = null;
        }

        m_interactionText.gameObject.SetActive(m_current != null);
    }

    private void HandleInput()
    {
        if (m_current == null) return;

        if (Input.GetKeyDown(m_key))
            m_current.StartDestroy();

        if (Input.GetKey(m_key))
            m_current.UpdateDestroy(Time.deltaTime);

        if (Input.GetKeyUp(m_key))
            m_current.CancelDestroy();
    }
}