using UnityEngine;

public class DestroyItem : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private float collectionTime = 3f;
    [SerializeField] private float interactionDistance = 5f;

    [Header("Звуки")]
    [SerializeField] private AudioClip collectingSound;
    [SerializeField] private AudioClip completeSound;

    [Header("Эффекты частиц")]
    [SerializeField] private GameObject collectingParticlesPrefab; // ПРЕФАБ, а не готовый объект!
    [SerializeField] private GameObject completeParticlesPrefab;   // ПРЕФАБ!
    [SerializeField] private float particleOffsetY = 0.5f;

    // Компоненты
    private Camera playerCamera;
    private AudioSource audioSource;
    private Renderer objectRenderer;
    private Color originalColor;

    // Частицы (создаются для каждого объекта отдельно)
    private ParticleSystem collectingParticles;
    private ParticleSystem completeParticles;

    // Состояние
    private bool isCollecting = false;
    private float currentCollectionTime = 0f;
    private bool isHighlighted = false;

    void Start()
    {
        playerCamera = Camera.main;

        // Создаем AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 10f;

        // Получаем Renderer
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }

        // Создаем частицы для ЭТОГО объекта
        CreateParticlesForThisObject();
    }

    void CreateParticlesForThisObject()
    {
        // Уничтожаем старые частицы если есть (на всякий случай)
        DestroyOldParticles();

        // Создаем частицы сбора из префаба
        if (collectingParticlesPrefab != null)
        {
            GameObject collectingParticlesObj = Instantiate(
                collectingParticlesPrefab,
                transform.position + Vector3.up * particleOffsetY,
                Quaternion.identity
            );

            collectingParticlesObj.name = "CollectingParticles_" + gameObject.name;
            collectingParticlesObj.transform.SetParent(transform); // Делаем дочерним!

            collectingParticles = collectingParticlesObj.GetComponent<ParticleSystem>();
            collectingParticles.Stop();

            Debug.Log($"Созданы частицы сбора для {gameObject.name}");
        }

        // Создаем частицы завершения из префаба
        if (completeParticlesPrefab != null)
        {
            GameObject completeParticlesObj = Instantiate(
                completeParticlesPrefab,
                transform.position + Vector3.up * particleOffsetY,
                Quaternion.identity
            );

            completeParticlesObj.name = "CompleteParticles_" + gameObject.name;
            completeParticlesObj.transform.SetParent(transform); // Делаем дочерним!

            completeParticles = completeParticlesObj.GetComponent<ParticleSystem>();
            completeParticles.Stop();
        }
    }

    void DestroyOldParticles()
    {
        // Ищем и уничтожаем старые частицы в детях
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Particles"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    void Update()
    {
        // Проверяем, смотрит ли игрок на предмет
        bool isLooking = IsPlayerLookingAtObject();

        // Подсветка при наведении
        if (isLooking && !isHighlighted)
        {
            StartHighlight();
        }
        else if (!isLooking && isHighlighted)
        {
            StopHighlight();
        }

        // Обработка взаимодействия
        if (isLooking)
        {
            HandleInteraction();
        }
        else if (isCollecting)
        {
            StopCollecting();
        }
    }

    bool IsPlayerLookingAtObject()
    {
        if (playerCamera == null) return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        return Physics.Raycast(ray, out hit, interactionDistance) &&
               hit.collider.gameObject == this.gameObject;
    }

    void StartHighlight()
    {
        isHighlighted = true;

        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.Lerp(originalColor, Color.yellow, 0.3f);
        }
    }

    void StopHighlight()
    {
        isHighlighted = false;

        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isCollecting)
        {
            StartCollecting();
        }

        if (Input.GetKeyUp(KeyCode.E) && isCollecting)
        {
            StopCollecting();
        }

        if (Input.GetKey(KeyCode.E) && isCollecting)
        {
            UpdateCollecting();
        }
    }

    void StartCollecting()
    {
        isCollecting = true;
        currentCollectionTime = 0f;

        // Звук
        PlayCollectingSound(true);

        // Частицы
        if (collectingParticles != null)
        {
            collectingParticles.Play();
        }

        Debug.Log($"Начали собирать {gameObject.name}...");
    }

    void UpdateCollecting()
    {
        currentCollectionTime += Time.deltaTime;

        float progress = currentCollectionTime / collectionTime;
        Debug.Log($"{gameObject.name}: Прогресс {progress * 100:F0}%");

        // Меняем цвет
        if (objectRenderer != null)
        {
            Color progressColor = Color.Lerp(Color.yellow, Color.green, progress);
            objectRenderer.material.color = Color.Lerp(originalColor, progressColor, 0.5f);
        }

        // Обновляем частицы
        UpdateParticlesIntensity(progress);

        if (currentCollectionTime >= collectionTime)
        {
            CompleteCollecting();
        }
    }

    void StopCollecting()
    {
        if (!isCollecting) return;

        isCollecting = false;
        currentCollectionTime = 0f;

        PlayCollectingSound(false);

        if (collectingParticles != null)
        {
            collectingParticles.Stop();
        }

        if (objectRenderer != null && !isHighlighted)
        {
            objectRenderer.material.color = originalColor;
        }

        Debug.Log($"Сбор {gameObject.name} остановлен");
    }

    void CompleteCollecting()
    {
        isCollecting = false;

        PlayCollectingSound(false);

        if (collectingParticles != null)
        {
            collectingParticles.Stop();
        }

        StartCoroutine(CompleteEffect());
    }

    System.Collections.IEnumerator CompleteEffect()
    {
        // Звук завершения
        if (completeSound != null)
        {
            audioSource.PlayOneShot(completeSound);
        }

        // Частицы завершения
        if (completeParticles != null)
        {
            completeParticles.Play();
        }

        // Мерцание
        if (objectRenderer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                objectRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);
                objectRenderer.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
        }

        Debug.Log($"{gameObject.name} собран!");

        // Ждем завершения частиц
        if (completeParticles != null)
        {
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }

    void PlayCollectingSound(bool play)
    {
        if (collectingSound == null) return;

        if (play)
        {
            audioSource.clip = collectingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    void UpdateParticlesIntensity(float progress)
    {
        if (collectingParticles == null) return;

        var emission = collectingParticles.emission;
        var main = collectingParticles.main;

        emission.rateOverTime = Mathf.Lerp(10f, 50f, progress);
        main.startColor = Color.Lerp(Color.yellow, Color.green, progress);
    }

    void OnDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}