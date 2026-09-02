using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    // For use on the testing NPC, should be replaced with the logic in WorldItem and its link to the InteractionAbility

    public void Interact()
    {
        Debug.Log($"Interaction with {gameObject}");
    }
}
