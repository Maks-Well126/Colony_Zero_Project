using UnityEngine;
using Player;

public class RobotArmor : MonoBehaviour
{
    [SerializeField] private RobotController m_robot;
    private void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
           MouseDirectionChecker.GetMouseDirection(0.3f, 50);

        if (direction == MouseDirectionChecker.MouseDirection.Left && Input.GetMouseButtonDown(0))
        {
            m_robot.UpgradeHealth();
        }
        
    }
    
}
