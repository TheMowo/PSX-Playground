using UnityEngine;

public class DialogueMenuButtons : MonoBehaviour
{
    public void Button_AcceptDelivery()
    {
        M_DialogueManager.Instance.AcceptDelivery();
    }

    public void Button_Leave()
    {
        M_DialogueManager.Instance.EndDialogue();
    }
}