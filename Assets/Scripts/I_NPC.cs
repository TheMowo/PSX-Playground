using UnityEngine;

public class I_NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName;
    [SerializeField] private ItemData packageItem;
    [SerializeField] private Transform testObjectiveTransform;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;

    public void Interact()
    {
        Debug.Log($"Interaction with {npcName} received");

        // Open dialogue box
        dialogueBox.SetActive(true);

        // Lock camera (so player can’t move view during dialogue)
        ThirdPersonCamera.Instance.UnlockCamera();

        // Disable player interaction ability while dialogue is open
        PlayerPublicRef.Instance.gameObject.GetComponent<InteractionAbility>().enabled = false;
    }

    public void Button_AcceptDelivery()
    {
        Debug.Log($"{npcName}: Delivery Accepted!");

        // Add package item to inventory
        M_InventoryManager.Instance.AddItem(packageItem, 1);

        // Start timer and set destination
        M_ObjectiveManager.Instance.StartTimer(2f, 0f); // example: 2 minutes
        M_ObjectiveManager.Instance.SetDestination(testObjectiveTransform);

        // Close dialogue
        Button_Leave();
    }

    public void Button_Leave()
    {
        dialogueBox?.SetActive(false);

        // Re-lock camera
        ThirdPersonCamera.Instance.LockCamera();

        // Re-enable interaction ability
        PlayerPublicRef.Instance.gameObject.GetComponent<InteractionAbility>().enabled = true;
    }
}
