using UnityEngine;
using TMPro;
using System.Collections;

public class DialogeSystem : MonoBehaviour
{
    public enum DialogType
    {
        Artef1Delivered,
        GameStart,
        Artef2Delivered,
        Build,
        Replica5,
        Replica6,
        Replica7,
        Replica8,
        Replica9,
        Replica10,
        Replica11
    }

    [System.Serializable]
    public struct DialogData
    {
        public DialogType type;
        [TextArea] public string text;
        public AudioClip clip;
    }

    [SerializeField] private TextMeshProUGUI m_dialogText;
    [SerializeField] private float m_speedText = 0.05f;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private DialogData[] m_dialogs = new DialogData[11];

    private static DialogeSystem m_instance;
    private Coroutine m_currentCoroutine;

    private void Awake()
    {
        m_instance = this;
    }   
    public static void StartDialoge(DialogType type)
    {
        if (m_instance == null) return;

        foreach (var dialog in m_instance.m_dialogs)
        {
            if (dialog.type == type)
            {
                m_instance.PlayDialog(dialog);
                return;
            }
        }
    }

    private void PlayDialog(DialogData data)
    {
        if (m_currentCoroutine != null)
            StopCoroutine(m_currentCoroutine);

        StopAllCoroutines();

        m_dialogText.text = "";

        if (data.clip != null)
        {
            m_audioSource.Stop();
            m_audioSource.clip = data.clip;
            m_audioSource.Play();
        }

        m_currentCoroutine = StartCoroutine(WriteSentence(data.text));
        StartCoroutine(ClearAfterAudio());
    }

    private IEnumerator WriteSentence(string sentence)
    {
        foreach (char c in sentence)
        {
            m_dialogText.text += c;
            yield return new WaitForSeconds(m_speedText);
        }
    }

    private IEnumerator ClearAfterAudio()
    {
        if (m_audioSource.clip == null)
            yield break;

        yield return new WaitWhile(() => m_audioSource.isPlaying);

        m_dialogText.text = "";
    }
}