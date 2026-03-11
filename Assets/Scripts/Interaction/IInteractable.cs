public interface IInteractable
{
    string GetInteractionText();

    void StartInteract();
    void UpdateInteract(float deltaTime);
    void CancelInteract();
}