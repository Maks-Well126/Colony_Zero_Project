using UnityEngine;

public class RobotWaySelect : MonoBehaviour
{
    [SerializeField] private GameObject m_wayPanell;

    private void Start()
    {
        m_wayPanell.gameObject.SetActive(false);
    }

    private void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
           MouseDirectionChecker.GetMouseDirection(0.3f, 50);

        if (direction == MouseDirectionChecker.MouseDirection.Right && Input.GetMouseButtonDown(0))
        {
            m_wayPanell.gameObject.SetActive(true);
        }
      //  if (Input.GetKeyDown(KeyCode.X))  m_wayPanell.gameObject.SetActive(false);
    }

    public void ExitButton()
    {
        m_wayPanell.gameObject.SetActive(false);
    }
}
