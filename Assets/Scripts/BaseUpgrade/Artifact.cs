using UnityEngine;

public class Artifact : MonoBehaviour
{
    public static bool m_isDelivered { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            m_isDelivered = true;
            Destroy(gameObject);
        }
    }
}
