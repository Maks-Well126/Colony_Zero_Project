using UnityEngine;

public class Base : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Artifact>())
        {
            Debug.Log("Артефакт доставлен на базу");
        }
    }
}
