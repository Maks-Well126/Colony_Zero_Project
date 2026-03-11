using UnityEngine;

public class RobotWaySelect : MonoBehaviour
{
    [SerializeField] private GameObject m_wayPanell;
    [SerializeField] private GameObject m_radial;

    private void Start()
    {
        m_wayPanell.gameObject.SetActive(false);
    }

    private void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
           MouseDirectionChecker.GetMouseDirection(0.3f, 50);

        if (direction == MouseDirectionChecker.MouseDirection.Right && Input.GetMouseButtonDown(0) && m_radial.gameObject.activeSelf)
        {
            m_wayPanell.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && m_wayPanell.gameObject.activeSelf) 
        {
            m_wayPanell.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }  
    }

    public void ExitButton()
    {
        m_wayPanell.gameObject.SetActive(false);
    }
}
