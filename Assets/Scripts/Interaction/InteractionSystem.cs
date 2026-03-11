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
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactLayer))
        {
            current = hit.collider.GetComponent<IInteractable>();
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