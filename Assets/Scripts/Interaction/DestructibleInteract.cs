using UnityEngine;

public class DestructibleInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Destructible destructible;

    private void Awake()
    {
        if (destructible == null)
            destructible = GetComponent<Destructible>();
    }

    public string GetInteractionText()
    {
        return "Hold E to clear obstacle";
    }

    public void StartInteract()
    {
        destructible.StartDestroy();
    }

    public void UpdateInteract(float deltaTime)
    {
        destructible.UpdateDestroy(deltaTime);
    }

    public void CancelInteract()
    {
        destructible.CancelDestroy();
    }
}