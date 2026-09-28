using UnityEngine;

public class I_NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName;
    //[SerializeField] private ItemData packageItem;

    [Header("Delivery Settings")]
    [SerializeField] private DeliveryData deliveryData;
    [SerializeField] private Transform deliveryDestination;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;

    public void Interact()
    {
        Debug.Log($"Interaction with {npcName} received");

        // Open dialogue box
        dialogueBox.SetActive(true);

        // Lock camera (so player can’t move view during dialogue)
        FirstPersonCamera.Instance.UnlockCamera();
        PlayerPublicRef.Instance.GetComponent<CharacterMotor>().DisableMovement();

        // Disable player interaction ability while dialogue is open
        PlayerPublicRef.Instance.gameObject.GetComponent<InteractionAbility>().enabled = false;
    }

    public void Button_AcceptDelivery()
    {
        Transform destination = M_DeliveryLocationManager.Instance.GetRandomDestination();

        M_DeliveryManager.Instance.StartDelivery(deliveryData, destination);

        Button_Leave();
    }

    public void Button_Leave()
    {
        dialogueBox?.SetActive(false);

        // Re-lock camera
        FirstPersonCamera.Instance.LockCamera();
        PlayerPublicRef.Instance.GetComponent<CharacterMotor>().EnableMovement();

        // Re-enable interaction ability
        PlayerPublicRef.Instance.gameObject.GetComponent<InteractionAbility>().enabled = true;
    }
}
