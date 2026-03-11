using UnityEngine;

public class RobotRepairInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private RobotRepair repair;

    private void Awake()
    {
        if (repair == null)
            repair = GetComponent<RobotRepair>();
    }

    public string GetInteractionText()
    {
        // only show interact text when the robot actually needs repair
        if (repair == null || !repair.CanRepair)
            return string.Empty;
        return "Hold E to repair robot";
    }

    public void StartInteract()
    {
        repair.StartRepair();
    }

    public void UpdateInteract(float deltaTime)
    {
        repair.UpdateRepair(deltaTime);
    }

    public void CancelInteract()
    {
        repair.CancelRepair();
    }
}