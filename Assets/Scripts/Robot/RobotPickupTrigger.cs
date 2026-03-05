using UnityEngine;

public class RobotPickupTrigger : MonoBehaviour
{
    public System.Action<GameObject> OnArtifactPick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Artifact"))
        {
            OnArtifactPick?.Invoke(other.gameObject);
        }
    }
}