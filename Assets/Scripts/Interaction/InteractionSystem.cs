using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [SerializeField] private TMP_Text interactionText;

    private Camera cam;
    private IInteractable current;

    private void Start()
    {
        cam = Camera.main;
        interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckInteractable();
        HandleInput();
    }

    private void CheckInteractable()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin += ray.direction * 0.1f;

        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                current = null;
            }
            else
            {
                current = hit.collider.GetComponent<IInteractable>()
                          ?? hit.collider.GetComponentInParent<IInteractable>();
            }
        }
        else
        {
            current = null;
        }

        if (current != null)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = current.GetInteractionText();
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private void HandleInput()
    {
        if (current == null) return;

        if (Input.GetKeyDown(interactKey))
            current.StartInteract();

        if (Input.GetKey(interactKey))
            current.UpdateInteract(Time.deltaTime);

        if (Input.GetKeyUp(interactKey))
            current.CancelInteract();
    }
}