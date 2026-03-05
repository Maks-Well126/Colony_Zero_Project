using UnityEngine;

public class RobotPlayerRangeTrigger : MonoBehaviour
{
    public System.Action<bool> OnPlayerRangeChanged;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnPlayerRangeChanged?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            OnPlayerRangeChanged?.Invoke(false);
    }
}