using UnityEngine;

public class DinoTrigger : MonoBehaviour
{
    [SerializeField] private DinoBackgroundSequence dino;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dino.StartSequence();
            gameObject.SetActive(false);
        }
    }
}