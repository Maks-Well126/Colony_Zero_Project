using UnityEngine;
using static DialogeSystem;

public class Artifact : MonoBehaviour
{
    public static bool isArtefact1Delivered = false;
    public static bool isArtefact2Delivered = false;

    [SerializeField] private Artifact m_artifact1;
    [SerializeField] private Artifact m_artifact2;


    private static int m_deliveredCount = 0;
    private void Start()
    {
        if (Save.LoadLevel1State()) 
        { 
            Destroy(m_artifact1.gameObject);
            m_deliveredCount++;
        }
        if (Save.LoadLevel2State()) Destroy(m_artifact2.gameObject);

    }   
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Base"))
            return;

        AudioManager.Instance.PlayMusic(0);

        if (m_deliveredCount == 0)
        {
            isArtefact1Delivered = true;
            m_deliveredCount++;
            Save.SaveLevel1State(true);
            Destroy(gameObject, 0.1f);
            DialogeSystem.StartDialoge(DialogType.Artef1Delivered);
            return;
        }

        if (m_deliveredCount >= 1)
        {
            isArtefact2Delivered = true;
            m_deliveredCount++;
            Save.SaveLevel2State(true);
            DialogeSystem.StartDialoge(DialogType.Artef2Delivered);
            Destroy(gameObject, 0.1f);
            return;
        }
    }
}
