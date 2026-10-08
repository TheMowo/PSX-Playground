using UnityEngine;

public class M_DialogueManager : MonoBehaviour
{
    public static M_DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialogueBox;

    private I_NPC currentNPC;

    public bool IsDialogueOpen => dialogueBox != null && dialogueBox.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(I_NPC npc)
    {
        currentNPC = npc;

        dialogueBox.SetActive(true);

        M_GameManager.Instance.AddPauseReason(M_GameManager.PauseReason.Dialogue);

        PlayerPublicRef.Instance.GetComponent<InteractionAbility>().enabled = false;
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);

        currentNPC = null;

        M_GameManager.Instance.RemovePauseReason(
            M_GameManager.PauseReason.Dialogue);

        PlayerPublicRef.Instance
            .GetComponent<InteractionAbility>()
            .enabled = true;
    }

    public void AcceptDelivery()
    {
        if (currentNPC == null)
            return;

        Transform destination =
            M_DeliveryLocationManager.Instance
            .GetRandomDestination();

        M_DeliveryManager.Instance.StartDelivery(
            currentNPC.DeliveryData,
            destination);

        EndDialogue();
    }
}