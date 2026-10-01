using UnityEngine;

public class I_NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName;

    [Header("Delivery Settings")]
    [SerializeField] private DeliveryData deliveryData;

    public string NPCName => npcName;

    public DeliveryData DeliveryData => deliveryData;

    public void Interact()
    {
        Debug.Log($"Interaction with {npcName} received");

        M_DialogueManager.Instance.StartDialogue(this);
    }
}