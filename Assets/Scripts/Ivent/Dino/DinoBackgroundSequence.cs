using UnityEngine;
using System.Collections;
using Player;
using UnityEngine.Audio;

public class DinoBackgroundSequence : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Camera")]
    [SerializeField] private PlayerCameraController cameraController;

    [Header("Audio")]
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip roarSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private AudioSource m_audioSource;
    private bool m_started;

    private void Awake()
{
    m_audioSource = gameObject.AddComponent<AudioSource>();

    if (sfxMixerGroup != null)
        m_audioSource.outputAudioMixerGroup = sfxMixerGroup;

    transform.position = pointA.position;
    transform.LookAt(pointB);

    if (walkAudioSource != null)
    {
        walkAudioSource.loop = true;
        walkAudioSource.clip = walkSound;
        walkAudioSource.playOnAwake = false;

        if (sfxMixerGroup != null)
            walkAudioSource.outputAudioMixerGroup = sfxMixerGroup;
    }
}

    public void StartSequence()
    {
        if (m_started)
            return;

        m_started = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        animator.SetTrigger("Roar");

        if (roarSound != null)
            m_audioSource.PlayOneShot(roarSound);

        StartCoroutine(StepShake(0.08f, 1.5f));

        yield return new WaitForSeconds(3f);

        animator.SetTrigger("Walk");

        if (walkAudioSource != null && !walkAudioSource.isPlaying)
        {
            walkAudioSource.pitch = Random.Range(0.95f, 1.05f);
            walkAudioSource.Play();
        }

        StartCoroutine(WalkShake());

        while (Vector3.Distance(transform.position, pointB.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                pointB.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        animator.SetTrigger("Die");

        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }
    }

    private IEnumerator WalkShake()
    {
        while (true)
        {
            StartCoroutine(StepShake(0.05f, 0.25f));
            yield return new WaitForSeconds(1.4f);
        }
    }

    private IEnumerator StepShake(float intensity, float duration)
    {
        Transform cam = cameraController.transform;

        Vector3 originalPos = cam.localPosition;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float offset = Mathf.Sin(timer * 25f) * intensity;

            cam.localPosition = originalPos + new Vector3(0f, offset, 0f);

            yield return null;
        }

        cam.localPosition = originalPos;
    }
    public void OnDinoImpact()
    {
        StartCoroutine(StepShake(0.25f, 0.5f));

        if (deathSound != null)
            m_audioSource.PlayOneShot(deathSound);

        Invoke(nameof(DisableDino), 2f);
    }

    private void DisableDino()
    {
        gameObject.SetActive(false);
    }
}