using UnityEngine;

public class TriggerObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;

    private void Start()
    {
        if (objectToToggle != null)
            objectToToggle.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objectToToggle != null)
        {
            objectToToggle.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && objectToToggle != null)
        {
            objectToToggle.SetActive(false);
        }
    }
}