using Ink.Runtime;
using UnityEngine;
using System;

public class M_DialogueManager : Singleton<M_DialogueManager>
{
    public event Action<I_NPC> OnEnterDialogue;
    public event Action DialogueEnded;

    private Story currentStory;
    private I_NPC currentNPC;

    public bool IsDialogueOpen => currentStory != null;

    public Story CurrentStory => currentStory;

    public I_NPC CurrentNPC => currentNPC;

    public void StartDialogue(I_NPC npc)
    {
        if (IsDialogueOpen)
            return;

        if (npc == null || npc.InkJSON == null)
            return;

        currentNPC = npc;
        currentStory = new Story(npc.InkJSON.text);

        BindExternalFunctions();

        UI_Dialogue.Instance.Show();

        M_GameManager.Instance.AddPauseReason(M_GameManager.PauseReason.Dialogue);

        InteractionAbility interactionAbility = PlayerPublicRef.Instance.GetComponent<InteractionAbility>();

        interactionAbility.BlockInteraction();
        interactionAbility.enabled = false;

        OnEnterDialogue?.Invoke(currentNPC);

        ContinueStory();
    }

    private void BindExternalFunctions()
    {
        currentStory.BindExternalFunction(
            "StartDelivery",
            StartDelivery);
    }

    private void UnbindExternalFunctions()
    {
        if (currentStory == null)
            return;

        currentStory.UnbindExternalFunction("StartDelivery");
    }

    private void StartDelivery()
    {
        if (currentNPC == null)
            return;

        Transform destination = M_DeliveryLocationManager.Instance.GetRandomDestination();

        M_DeliveryManager.Instance.StartDelivery(currentNPC.DeliveryData, destination);
    }

    public void ContinueStory()
    {
        //Debug.Log("ContinueStory Called");
        //Debug.Log($"canContinue: {currentStory.canContinue}");
        //Debug.Log($"choices: {currentStory.currentChoices.Count}");

        if (currentStory == null)
            return;

        if (currentStory.canContinue)
        {
            string line = currentStory.Continue().Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                ContinueStory();
                return;
            }

            UI_Dialogue.Instance.ShowLine(currentNPC.NPCName, line);

            return;
        }

        if (currentStory.currentChoices.Count > 0)
        {
            UI_Dialogue.Instance.ShowChoices(currentStory.currentChoices);

            return;
        }

        EndDialogue();
    }

    public void SelectChoice(int choiceIndex)
    {
        Debug.Log($"Choice Selected: {choiceIndex}");

        if (currentStory == null)
            return;

        currentStory.ChooseChoiceIndex(choiceIndex);

        ContinueStory();
    }

    public void EndDialogue()
    {
        //Debug.Log("END DIALOGUE called");

        UI_Dialogue.Instance.Hide();

        UnbindExternalFunctions();

        currentStory = null;
        currentNPC = null;

        M_GameManager.Instance.RemovePauseReason(M_GameManager.PauseReason.Dialogue);

        InteractionAbility interactionAbility = PlayerPublicRef.Instance.GetComponent<InteractionAbility>();

        interactionAbility.enabled = true;
        interactionAbility.BlockInteraction();

        DialogueEnded?.Invoke();
    }
}