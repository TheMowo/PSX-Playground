using UnityEngine;

public class I_NPC : MonoBehaviour, IInteractable
{
    [Header("NPC")]
    [SerializeField] private string npcName;

    [Header("Dialogue")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Delivery")]
    [SerializeField] private DeliveryData deliveryData;

    public string NPCName => npcName;
    public TextAsset InkJSON => inkJSON;
    public DeliveryData DeliveryData => deliveryData;

    public void Interact()
    {
        M_DialogueManager.Instance.StartDialogue(this);
    }
}