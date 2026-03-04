using UnityEngine;
using Player;

public class Hill : MonoBehaviour
{
    [SerializeField] private PlayerController m_player;
    [SerializeField] private RobotController m_robot;

    private void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
           MouseDirectionChecker.GetMouseDirection(0.3f, 50);

        if (direction == MouseDirectionChecker.MouseDirection.Down && Input.GetMouseButtonDown(0))
        {
            Debug.Log("ssssssss");
            m_player.HealToFull();
            m_robot.RepairRobotAtBase();
        }
        
    }
    
}
