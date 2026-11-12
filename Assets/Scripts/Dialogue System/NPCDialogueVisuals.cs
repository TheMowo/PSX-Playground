using System.Collections;
using UnityEngine;
using Birno;

public class NPCDialogueVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SquashAndStretch squashAndStretch;

    [Header("Sprites")]
    [SerializeField] private Sprite mouthClosedSprite;
    [SerializeField] private Sprite mouthOpenSprite;

    [Header("Talking")]
    [SerializeField] private float minTalkRate = 0.05f;
    [SerializeField] private float maxTalkRate = 0.12f;

    private Coroutine talkingRoutine;

    public void StartTalking()
    {
        if (spriteRenderer == null)
            return;

        StopTalking();

        talkingRoutine = StartCoroutine(TalkingRoutine());
    }

    public void StopTalking()
    {
        if (talkingRoutine != null)
        {
            StopCoroutine(talkingRoutine);
            talkingRoutine = null;
        }

        if (spriteRenderer == null)
            return;

        if (mouthClosedSprite != null)
            spriteRenderer.sprite = mouthClosedSprite;
    }

    private IEnumerator TalkingRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        bool mouthOpen = false;

        while (true)
        {
            mouthOpen = !mouthOpen;

            spriteRenderer.sprite =
                mouthOpen
                ? mouthOpenSprite
                : mouthClosedSprite;

            if (mouthOpen && squashAndStretch != null)
                squashAndStretch.PlaySquashAndStretch();

            yield return new WaitForSecondsRealtime(
                Random.Range(minTalkRate, maxTalkRate));
        }
    }
}