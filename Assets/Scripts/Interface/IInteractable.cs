public interface IInteractable
{
    void StartInteraction();
    void UpdateInteraction(float deltaTime);
    void CancelInteraction();
}
