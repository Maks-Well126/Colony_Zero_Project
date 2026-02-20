using UnityEngine;

public class Artifact : MonoBehaviour
{
    public static bool isArtefact1Delivered = false;
    public static bool isArtefact2Delivered = false;

    private static int m_deliveredCount = 0; // общий счётчик для всех артефактов

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Base"))
            return;

        // First delivery
        if (m_deliveredCount == 0)
        {
            isArtefact1Delivered = true;
            m_deliveredCount++;
            Destroy(gameObject);
            return; // обязательно прерываем
        }

        // Second delivery
        if (m_deliveredCount == 1)
        {
            isArtefact2Delivered = true;
            m_deliveredCount++;
            Destroy(gameObject);
            return;
        }
    }
}
